using AutoMapper;
using ClosedXML.Excel;
using Dal.Interfaces;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.DTOs;
using Service.Exceptions;
using Service.Interfaces;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Service.Services
{
    public class CompanyService(
        ICompanyRepository companyRepository,
        IAddressRepository addressRepository,
        ICompanyAddressRepository companyAddressRepository,
        IMapper mapper,
        ProjectContext context,
        ILogger<CompanyService> logger) : ICompanyService
    {
        private static readonly StringComparer NameComparer = StringComparer.OrdinalIgnoreCase;

        public async Task<IEnumerable<CompanyDTO>> GetAllAsync()
        {
            var companies = await companyRepository.GetAllAsync();
            return mapper.Map<IEnumerable<CompanyDTO>>(companies);
        }

        public async Task<IEnumerable<AddressDTO>> GetAddressesAsync(int id)
        {
            var addresses = await companyRepository.GetAddressesAsync(id);
            return mapper.Map<IEnumerable<AddressDTO>>(addresses);
        }

        public async Task<CompanyDTO> CreateAsync(CreateCompanyDTO dto)
        {
            var name = NormalizeName(dto.Name);
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidDataException("Company name is required.");

            var existing = await context.Companies.FirstOrDefaultAsync(c => c.Name == name);
            if (existing?.IsActive == true)
                throw new ConflictException("Company with this name already exists.");

            if (existing != null)
            {
                existing.IsActive = true;
                await context.SaveChangesAsync();
                return mapper.Map<CompanyDTO>(existing);
            }

            var entity = new Company { Name = name, IsActive = true };
            await companyRepository.AddAsync(entity);
            return mapper.Map<CompanyDTO>(entity);
        }

        public async Task UpdateAsync(int id, UpdateCompanyDTO dto)
        {
            var entity = await companyRepository.GetByIdAsync(id);
            if (entity == null)
                return;

            var name = NormalizeName(dto.Name);
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidDataException("Company name is required.");

            if (await context.Companies.AnyAsync(c => c.Id != id && c.Name == name))
                throw new ConflictException("Company with this name already exists.");

            entity.Name = name;
            await companyRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var company = await context.Companies.FindAsync(id);
            if (company == null)
                return;

            var links = await context.CompanyAddresses
                .Where(link => link.CompanyId == id)
                .ToListAsync();
            context.CompanyAddresses.RemoveRange(links);

            if (await context.PumpOperations.AnyAsync(operation => operation.CompanyId == id))
                company.IsActive = false;
            else
                context.Companies.Remove(company);

            await context.SaveChangesAsync();
        }

        public async Task<AddressDTO> AddAddressAsync(int companyId, CreateAddressDTO dto)
        {
            var name = NormalizeName(dto.Name);
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidDataException("Address is required.");

            if (!await context.Companies.AnyAsync(c => c.Id == companyId && c.IsActive))
                throw new InvalidDataException("Company was not found.");

            var address = await context.Addresses.FirstOrDefaultAsync(a => a.Name == name);
            if (address == null)
            {
                address = new Address { Name = name, IsActive = true };
                await addressRepository.AddAsync(address);
            }
            else if (!address.IsActive)
            {
                address.IsActive = true;
                await context.SaveChangesAsync();
            }

            var linkExists = await context.CompanyAddresses
                .AnyAsync(ca => ca.CompanyId == companyId && ca.AddressId == address.Id);

            if (linkExists)
                throw new ConflictException("This address is already linked to the company.");

            await companyAddressRepository.AddAsync(new CompanyAddress
            {
                CompanyId = companyId,
                AddressId = address.Id
            });

            return mapper.Map<AddressDTO>(address);
        }

        public async Task UpdateAddressAsync(int addressId, UpdateAddressDTO dto)
        {
            var entity = await addressRepository.GetByIdAsync(addressId);
            if (entity == null)
                return;

            var name = NormalizeName(dto.Name);
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidDataException("Address is required.");

            if (await context.Addresses.AnyAsync(a => a.Id != addressId && a.Name == name))
                throw new ConflictException("Address with this name already exists.");

            entity.Name = name;
            await addressRepository.UpdateAsync(entity);
        }

        public async Task RemoveAddressAsync(int companyId, int addressId)
        {
            var link = await context.CompanyAddresses
                .FirstOrDefaultAsync(ca => ca.CompanyId == companyId && ca.AddressId == addressId);

            if (link == null)
                return;

            context.CompanyAddresses.Remove(link);

            var hasOtherLinks = await context.CompanyAddresses
                .AnyAsync(ca => ca.Id != link.Id && ca.AddressId == addressId);
            if (!hasOtherLinks)
            {
                var address = await context.Addresses.FindAsync(addressId);
                if (address != null)
                {
                    if (await context.PumpOperations.AnyAsync(operation => operation.AddressId == addressId))
                        address.IsActive = false;
                    else
                        context.Addresses.Remove(address);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<CompanyExcelImportResultDTO> ImportExcelAsync(Stream excelStream)
        {
            var stopwatch = Stopwatch.StartNew();
            logger.LogInformation("Company Excel import started.");

            ParsedExcelData parsed;
            try
            {
                parsed = ParseExcel(excelStream);
                logger.LogInformation(
                    "Company Excel parsed: {TotalRows} rows, {UniquePairs} unique pairs, {DuplicateRows} duplicate rows in {ElapsedMilliseconds} ms.",
                    parsed.TotalRows,
                    parsed.Rows.Count,
                    parsed.DuplicateRowsSkipped,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (InvalidDataException ex)
            {
                logger.LogWarning(ex, "Company Excel import validation failed after {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
                throw;
            }

            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var allCompanies = await context.Companies.ToListAsync();
                var allAddresses = await context.Addresses.ToListAsync();
                var companiesByName = allCompanies
                    .GroupBy(company => NormalizeName(company.Name), NameComparer)
                    .ToDictionary(group => group.Key, group => group.First(), NameComparer);

                var addressesByName = allAddresses
                    .GroupBy(address => NormalizeName(address.Name), NameComparer)
                    .ToDictionary(group => group.Key, group => group.First(), NameComparer);

                var requestedCompanies = parsed.Rows
                    .Select(row => row.CompanyName)
                    .Distinct(NameComparer)
                    .ToList();
                var requestedAddresses = parsed.Rows
                    .Select(row => row.AddressName)
                    .Distinct(NameComparer)
                    .ToList();

                var newCompanies = requestedCompanies
                    .Where(name => !companiesByName.ContainsKey(name))
                    .Select(name => new Company { Name = name, IsActive = true })
                    .ToList();
                var newAddresses = requestedAddresses
                    .Where(name => !addressesByName.ContainsKey(name))
                    .Select(name => new Address { Name = name, IsActive = true })
                    .ToList();

                foreach (var company in allCompanies)
                    company.IsActive = false;
                foreach (var address in allAddresses)
                    address.IsActive = false;

                foreach (var name in requestedCompanies.Where(companiesByName.ContainsKey))
                    companiesByName[name].IsActive = true;
                foreach (var name in requestedAddresses.Where(addressesByName.ContainsKey))
                    addressesByName[name].IsActive = true;

                if (newCompanies.Count > 0)
                    context.Companies.AddRange(newCompanies);
                if (newAddresses.Count > 0)
                    context.Addresses.AddRange(newAddresses);

                await context.SaveChangesAsync();

                foreach (var company in newCompanies)
                    companiesByName.Add(company.Name, company);
                foreach (var address in newAddresses)
                    addressesByName.Add(address.Name, address);

                var oldLinks = await context.CompanyAddresses.ToListAsync();
                context.CompanyAddresses.RemoveRange(oldLinks);
                await context.SaveChangesAsync();

                var linksToCreate = new List<CompanyAddress>();

                foreach (var row in parsed.Rows)
                {
                    var companyId = companiesByName[row.CompanyName].Id;
                    var addressId = addressesByName[row.AddressName].Id;

                    linksToCreate.Add(new CompanyAddress
                    {
                        CompanyId = companyId,
                        AddressId = addressId
                    });
                }

                if (linksToCreate.Count > 0)
                {
                    context.CompanyAddresses.AddRange(linksToCreate);
                    await context.SaveChangesAsync();
                }

                var usedCompanyIds = (await context.PumpOperations
                    .Select(operation => operation.CompanyId)
                    .Distinct()
                    .ToListAsync())
                    .ToHashSet();
                var usedAddressIds = (await context.PumpOperations
                    .Select(operation => operation.AddressId)
                    .Distinct()
                    .ToListAsync())
                    .ToHashSet();

                var companiesToDelete = allCompanies
                    .Where(company => !company.IsActive && !usedCompanyIds.Contains(company.Id))
                    .ToList();
                var addressesToDelete = allAddresses
                    .Where(address => !address.IsActive && !usedAddressIds.Contains(address.Id))
                    .ToList();
                context.Companies.RemoveRange(companiesToDelete);
                context.Addresses.RemoveRange(addressesToDelete);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                logger.LogInformation(
                    "Company Excel replacement completed in {ElapsedMilliseconds} ms: {Companies} active companies, {Addresses} active addresses, {Links} links; {RemovedLinks} old links replaced.",
                    stopwatch.ElapsedMilliseconds,
                    requestedCompanies.Count,
                    requestedAddresses.Count,
                    linksToCreate.Count,
                    oldLinks.Count);

                return new CompanyExcelImportResultDTO
                {
                    TotalRows = parsed.TotalRows,
                    UniquePairs = parsed.Rows.Count,
                    DuplicateRowsSkipped = parsed.DuplicateRowsSkipped,
                    CreatedCompanies = newCompanies.Count,
                    ExistingCompanies = requestedCompanies.Count - newCompanies.Count,
                    CreatedAddresses = newAddresses.Count,
                    ExistingAddresses = requestedAddresses.Count - newAddresses.Count,
                    CreatedLinks = linksToCreate.Count,
                    ExistingLinks = 0
                };
            }
            catch (DbUpdateException)
            {
                throw new ConflictException("იმპორტის დროს მონაცემებთან კონფლიქტი მოხდა. გთხოვთ, თავიდან სცადოთ.");
            }
        }

        private static ParsedExcelData ParseExcel(Stream excelStream)
        {
            try
            {
                using var workbook = new XLWorkbook(excelStream);
                var worksheet = workbook.Worksheets.FirstOrDefault()
                    ?? throw new InvalidDataException("Excel ფაილში სამუშაო ფურცელი არ არის.");
                var firstRow = worksheet.FirstRowUsed()
                    ?? throw new InvalidDataException("Excel ფაილი ცარიელია.");

                var headerRowNumber = firstRow.RowNumber();
                var firstHeader = NormalizeName(worksheet.Cell(headerRowNumber, 1).GetString());
                var secondHeader = NormalizeName(worksheet.Cell(headerRowNumber, 2).GetString());
                if (string.IsNullOrWhiteSpace(firstHeader) || string.IsNullOrWhiteSpace(secondHeader))
                    throw new InvalidDataException("პირველ სტრიქონში A და B სვეტების სათაურები უნდა იყოს მითითებული.");

                var lastRowNumber = worksheet.LastRowUsed()?.RowNumber() ?? headerRowNumber;
                var rows = new List<ExcelCompanyAddressRow>();
                var uniquePairs = new HashSet<string>(NameComparer);
                var invalidRows = new List<int>();
                var duplicateRowsSkipped = 0;
                var totalRows = 0;

                for (var rowNumber = headerRowNumber + 1; rowNumber <= lastRowNumber; rowNumber++)
                {
                    var companyName = NormalizeName(worksheet.Cell(rowNumber, 1).GetString());
                    var addressName = NormalizeName(worksheet.Cell(rowNumber, 2).GetString());

                    if (string.IsNullOrWhiteSpace(companyName) && string.IsNullOrWhiteSpace(addressName))
                        continue;

                    if (string.IsNullOrWhiteSpace(addressName) && IsSummaryRow(companyName))
                        continue;

                    totalRows++;
                    if (string.IsNullOrWhiteSpace(companyName) || string.IsNullOrWhiteSpace(addressName) ||
                        companyName.Length > 100 || addressName.Length > 100)
                    {
                        invalidRows.Add(rowNumber);
                        continue;
                    }

                    var pairKey = $"{companyName}\u001f{addressName}";
                    if (!uniquePairs.Add(pairKey))
                    {
                        duplicateRowsSkipped++;
                        continue;
                    }

                    rows.Add(new ExcelCompanyAddressRow(companyName, addressName));
                }

                if (invalidRows.Count > 0)
                {
                    var listedRows = string.Join(", ", invalidRows.Take(20));
                    var suffix = invalidRows.Count > 20 ? " და სხვა" : string.Empty;
                    throw new InvalidDataException(
                        $"შეცდომაა სტრიქონებში: {listedRows}{suffix}. A სვეტში კომპანია, ხოლო B სვეტში მისამართი აუცილებლად უნდა იყოს მითითებული. თითოეული მნიშვნელობა მაქსიმუმ 100 სიმბოლოს უნდა შეიცავდეს.");
                }

                if (rows.Count == 0)
                    throw new InvalidDataException("Excel ფაილში კომპანია-მისამართის არცერთი ჩანაწერი არ არის.");

                return new ParsedExcelData(rows, totalRows, duplicateRowsSkipped);
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("ფაილის წაკითხვა ვერ მოხერხდა. დარწმუნდით, რომ სწორ .xlsx ან .xlsm Excel ფაილს ტვირთავთ.", ex);
            }
        }

        private static string NormalizeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var normalized = value.Normalize(NormalizationForm.FormKC).Replace('\u00A0', ' ');
            return Regex.Replace(normalized, @"\s+", " ").Trim();
        }

        private static bool IsSummaryRow(string companyName)
        {
            var label = companyName.Trim().TrimEnd(':').ToLowerInvariant();
            return label is "итого" or "total" or "ჯამი" or "სულ";
        }

        private sealed record ExcelCompanyAddressRow(string CompanyName, string AddressName);
        private sealed record ParsedExcelData(
            List<ExcelCompanyAddressRow> Rows,
            int TotalRows,
            int DuplicateRowsSkipped);
    }
}

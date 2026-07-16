namespace Service.DTOs
{
    public class CompanyExcelImportResultDTO
    {
        public int TotalRows { get; set; }
        public int UniquePairs { get; set; }
        public int DuplicateRowsSkipped { get; set; }
        public int CreatedCompanies { get; set; }
        public int ExistingCompanies { get; set; }
        public int CreatedAddresses { get; set; }
        public int ExistingAddresses { get; set; }
        public int CreatedLinks { get; set; }
        public int ExistingLinks { get; set; }
    }
}

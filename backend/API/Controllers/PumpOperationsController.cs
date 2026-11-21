using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PumpOperationsController(
        IPumpOperationService service,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<PumpOperationsController> logger) : ControllerBase
    {
        private const uint DefaultStyle = 0;
        private const uint HeaderStyle = 1;
        private const uint TextStyle = 2;
        private const uint DateStyle = 3;
        private const uint DecimalStyle = 4;
        private const uint IntegerStyle = 5;
        private const uint TotalTextStyle = 6;
        private const uint TotalDecimalStyle = 7;
        private const uint TotalIntegerStyle = 8;

        private static readonly string[] ExportHeaders =
        [
            "#",
            "თარიღი",
            "სტატუსი",
            "ოპერატორი",
            "კომპანია",
            "მისამართი",
            "სახ. ნომერი",
            "მილსადენის სიგრძე",
            "ბეტონი",
            "დამატებითი დგომა",
            "დაშლა/აწყობა",
            "მოცდენა",
            "დღიური",
            "შენიშვნა"
        ];

        private readonly record struct ExcelCell(object? Value, uint StyleIndex);

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<ActionResult<PumpOperationDto>> Create(CreatePumpOperationDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            PumpOperationDto result;
            try
            {
                result = await service.CreateAsync(dto, userId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            await SendReviewRequestEmailsAsync(result, await service.GetReviewRequestsForOperationAsync(result.Id));

            return Ok(result);
        }

        /// <summary>Edits the caller's own operation while it is still pending. Checkers get fresh review links.</summary>
        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<PumpOperationDto>> Update(int id, CreatePumpOperationDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            PumpOperationUpdateResult? result;
            try
            {
                result = await service.UpdateAsync(id, dto, userId);
            }
            catch (Service.Exceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            if (result == null) return NotFound();

            // Existing checkers keep their link and see the edit live; only checkers newly
            // added by a plate change need an email.
            await SendReviewRequestEmailsAsync(result.Operation, result.AddedReviewRequests, warnIfNoChecker: false);

            return Ok(result.Operation);
        }

        /// <summary>Deletes the caller's own operation while it is still pending.</summary>
        [Authorize(Roles = "Admin,User")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                return await service.DeleteAsync(id, userId) ? NoContent() : NotFound();
            }
            catch (Service.Exceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
        }

        private async Task SendReviewRequestEmailsAsync(
            PumpOperationDto result,
            IEnumerable<PumpOperationReviewRequestLinkDto> reviewRequests,
            bool warnIfNoChecker = true)
        {
            var reviewBaseUrl = GetReviewBaseUrl();
            var hasCheckerRequest = false;

            foreach (var reviewRequest in reviewRequests)
            {
                hasCheckerRequest = true;
                if (string.IsNullOrWhiteSpace(reviewRequest.CheckerGmail))
                {
                    // Checker has no email: they will see the request in the app after signing in.
                    continue;
                }

                var reviewUrl = $"{reviewBaseUrl}/{reviewRequest.Token}";
                try
                {
                    await emailService.SendPumpOperationReviewRequestAsync(result, reviewRequest.CheckerGmail, reviewUrl);

                    logger.LogInformation(
                        "Pump operation {PumpOperationId} review email processed for checker {CheckerGmail}: {ReviewUrl}",
                        result.Id,
                        reviewRequest.CheckerGmail,
                        reviewUrl);
                }
                catch (Exception ex)
                {
                    // The operation and review request are already persisted. An email-provider
                    // outage must not report the successful save as a failed operation.
                    logger.LogError(
                        ex,
                        "Pump operation {PumpOperationId} was saved, but the review email failed for checker {CheckerGmail}.",
                        result.Id,
                        reviewRequest.CheckerGmail);
                }
            }

            if (!hasCheckerRequest && warnIfNoChecker)
            {
                logger.LogWarning("Pump operation {PumpOperationId} was submitted, but no checker is configured.", result.Id);
            }
        }

        /// <summary>Review requests assigned to the signed-in user (pending first).</summary>
        [HttpGet("reviews/mine")]
        public async Task<ActionResult<IEnumerable<PumpOperationReviewDto>>> GetMyReviews()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var result = await service.GetReviewsForCheckerAsync(userId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("review/{token}")]
        public async Task<ActionResult<PumpOperationReviewDto>> GetReview(string token)
        {
            var result = await service.GetReviewByTokenAsync(token);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("review/{token}/accept")]
        public async Task<ActionResult<PumpOperationReviewDto>> AcceptReview(string token, [FromQuery] string? version)
        {
            return await SubmitDecisionAsync(token, "Accepted", version);
        }

        [AllowAnonymous]
        [HttpPost("review/{token}/reject")]
        public async Task<ActionResult<PumpOperationReviewDto>> RejectReview(string token, [FromQuery] string? version)
        {
            return await SubmitDecisionAsync(token, "Rejected", version);
        }

        /// <summary>409 with the current data when the operation was edited after the checker loaded <paramref name="version"/>.</summary>
        private async Task<ActionResult<PumpOperationReviewDto>> SubmitDecisionAsync(string token, string decision, string? version)
        {
            try
            {
                var result = await service.SubmitReviewDecisionAsync(token, decision, version);
                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Service.Exceptions.ReviewOutdatedException ex)
            {
                return Conflict(ex.Current);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PumpOperationDto>>> GetAll([FromQuery] PumpOperationFilterDto? filter)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var result = await service.GetAllAsync(userId, role, filter);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Accountant")]
        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] PumpOperationFilterDto? filter)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var operations = (await service.GetAllAsync(userId, role, filter)).ToList();
            var workbook = BuildPumpOperationsWorkbook(operations);
            var fileName = $"pump-operations-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";

            return File(
                workbook,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PumpOperationDto>> GetById(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";

            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var result = await service.GetByIdAsync(id, userId, role);
            if (result == null) return NotFound();
            return Ok(result);
        }

        private string GetReviewBaseUrl()
        {
            var configuredUrl = configuration["ReviewBaseUrl"];
            if (!string.IsNullOrWhiteSpace(configuredUrl))
            {
                return configuredUrl.TrimEnd('/');
            }

            var productionHost = Environment.GetEnvironmentVariable("VERCEL_PROJECT_PRODUCTION_URL");
            if (!string.IsNullOrWhiteSpace(productionHost))
            {
                return $"https://{productionHost.TrimEnd('/')}/review";
            }

            var origin = Request.Headers.Origin.ToString();
            if (!string.IsNullOrWhiteSpace(origin))
            {
                return $"{origin.TrimEnd('/')}/review";
            }

            return "http://localhost:4200/review";
        }

        private static byte[] BuildPumpOperationsWorkbook(IReadOnlyList<PumpOperationDto> operations)
        {
            using var stream = new MemoryStream();

            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
            {
                AddZipEntry(archive, "[Content_Types].xml", CreateContentTypesXml());
                AddZipEntry(archive, "_rels/.rels", CreatePackageRelationshipsXml());
                AddZipEntry(archive, "xl/workbook.xml", CreateWorkbookXml());
                AddZipEntry(archive, "xl/_rels/workbook.xml.rels", CreateWorkbookRelationshipsXml());
                AddZipEntry(archive, "xl/styles.xml", CreateStylesXml());
                AddZipEntry(archive, "xl/worksheets/sheet1.xml", CreatePumpOperationsSheetXml(operations));
            }

            return stream.ToArray();
        }

        private static XDocument CreateContentTypesXml()
        {
            XNamespace ns = "http://schemas.openxmlformats.org/package/2006/content-types";

            return new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "Types",
                    new XElement(ns + "Default",
                        new XAttribute("Extension", "rels"),
                        new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")),
                    new XElement(ns + "Default",
                        new XAttribute("Extension", "xml"),
                        new XAttribute("ContentType", "application/xml")),
                    new XElement(ns + "Override",
                        new XAttribute("PartName", "/xl/workbook.xml"),
                        new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml")),
                    new XElement(ns + "Override",
                        new XAttribute("PartName", "/xl/styles.xml"),
                        new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml")),
                    new XElement(ns + "Override",
                        new XAttribute("PartName", "/xl/worksheets/sheet1.xml"),
                        new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"))));
        }

        private static XDocument CreatePackageRelationshipsXml()
        {
            XNamespace ns = "http://schemas.openxmlformats.org/package/2006/relationships";

            return new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "Relationships",
                    new XElement(ns + "Relationship",
                        new XAttribute("Id", "rId1"),
                        new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"),
                        new XAttribute("Target", "xl/workbook.xml"))));
        }

        private static XDocument CreateWorkbookXml()
        {
            XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            XNamespace relationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

            return new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "workbook",
                    new XAttribute(XNamespace.Xmlns + "r", relationships.NamespaceName),
                    new XElement(ns + "sheets",
                        new XElement(ns + "sheet",
                            new XAttribute("name", "Pump Operations"),
                            new XAttribute("sheetId", "1"),
                            new XAttribute(relationships + "id", "rId1")))));
        }

        private static XDocument CreateWorkbookRelationshipsXml()
        {
            XNamespace ns = "http://schemas.openxmlformats.org/package/2006/relationships";

            return new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "Relationships",
                    new XElement(ns + "Relationship",
                        new XAttribute("Id", "rId1"),
                        new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"),
                        new XAttribute("Target", "worksheets/sheet1.xml")),
                    new XElement(ns + "Relationship",
                        new XAttribute("Id", "rId2"),
                        new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"),
                        new XAttribute("Target", "styles.xml"))));
        }

        private static XDocument CreateStylesXml()
        {
            XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            return new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "styleSheet",
                    new XElement(ns + "numFmts",
                        new XAttribute("count", "3"),
                        new XElement(ns + "numFmt",
                            new XAttribute("numFmtId", "164"),
                            new XAttribute("formatCode", "#,##0.00")),
                        new XElement(ns + "numFmt",
                            new XAttribute("numFmtId", "165"),
                            new XAttribute("formatCode", "#,##0")),
                        new XElement(ns + "numFmt",
                            new XAttribute("numFmtId", "166"),
                            new XAttribute("formatCode", "yyyy-mm-dd"))),
                    new XElement(ns + "fonts",
                        new XAttribute("count", "3"),
                        CreateFont(ns, size: 12),
                        CreateFont(ns, isBold: true, size: 13),
                        CreateFont(ns, isBold: true, size: 12)),
                    new XElement(ns + "fills",
                        new XAttribute("count", "4"),
                        new XElement(ns + "fill", new XElement(ns + "patternFill", new XAttribute("patternType", "none"))),
                        new XElement(ns + "fill", new XElement(ns + "patternFill", new XAttribute("patternType", "gray125"))),
                        CreateFill(ns, "FF92D050"),
                        CreateFill(ns, "FFFFFFFF")),
                    new XElement(ns + "borders",
                        new XAttribute("count", "2"),
                        CreateBorder(ns),
                        CreateBorder(ns, "FF000000", "medium")),
                    new XElement(ns + "cellStyleXfs",
                        new XAttribute("count", "1"),
                        new XElement(ns + "xf",
                            new XAttribute("numFmtId", "0"),
                            new XAttribute("fontId", "0"),
                            new XAttribute("fillId", "0"),
                            new XAttribute("borderId", "0"))),
                    new XElement(ns + "cellXfs",
                        new XAttribute("count", "9"),
                        CreateXf(ns),
                        CreateXf(ns, fontId: 1, fillId: 2, borderId: 1, alignment: "center", verticalAlignment: "center"),
                        CreateXf(ns, fillId: 3, borderId: 1, alignment: "left", verticalAlignment: "center"),
                        CreateXf(ns, fillId: 3, borderId: 1, numFmtId: 166, alignment: "center", verticalAlignment: "center"),
                        CreateXf(ns, fillId: 3, borderId: 1, numFmtId: 164, alignment: "right", verticalAlignment: "center"),
                        CreateXf(ns, fillId: 3, borderId: 1, numFmtId: 165, alignment: "right", verticalAlignment: "center"),
                        CreateXf(ns, fontId: 2, fillId: 2, borderId: 1, alignment: "left", verticalAlignment: "center"),
                        CreateXf(ns, fontId: 2, fillId: 2, borderId: 1, numFmtId: 164, alignment: "right", verticalAlignment: "center"),
                        CreateXf(ns, fontId: 2, fillId: 2, borderId: 1, numFmtId: 165, alignment: "right", verticalAlignment: "center")),
                    new XElement(ns + "cellStyles",
                        new XAttribute("count", "1"),
                        new XElement(ns + "cellStyle",
                            new XAttribute("name", "Normal"),
                            new XAttribute("xfId", "0"),
                            new XAttribute("builtinId", "0"))),
                    new XElement(ns + "dxfs", new XAttribute("count", "0")),
                    new XElement(ns + "tableStyles",
                        new XAttribute("count", "0"),
                        new XAttribute("defaultTableStyle", "TableStyleMedium9"),
                        new XAttribute("defaultPivotStyle", "PivotStyleLight16"))));
        }

        private static XElement CreateFont(
            XNamespace ns,
            bool isBold = false,
            double size = 11,
            string color = "FF0F172A")
        {
            var elements = new List<XElement>
            {
                new(ns + "sz", new XAttribute("val", size.ToString(CultureInfo.InvariantCulture))),
                new(ns + "color", new XAttribute("rgb", color)),
                new(ns + "name", new XAttribute("val", "Calibri"))
            };

            if (isBold)
            {
                elements.Insert(0, new XElement(ns + "b"));
            }

            return new XElement(ns + "font", elements);
        }

        private static XElement CreateFill(XNamespace ns, string color)
        {
            return new XElement(ns + "fill",
                new XElement(ns + "patternFill",
                    new XAttribute("patternType", "solid"),
                    new XElement(ns + "fgColor", new XAttribute("rgb", color)),
                    new XElement(ns + "bgColor", new XAttribute("indexed", "64"))));
        }

        private static XElement CreateBorder(XNamespace ns, string? color = null, string style = "thin")
        {
            XElement BorderPart(string name)
            {
                return color == null
                    ? new XElement(ns + name)
                    : new XElement(ns + name,
                        new XAttribute("style", style),
                        new XElement(ns + "color", new XAttribute("rgb", color)));
            }

            return new XElement(ns + "border",
                BorderPart("left"),
                BorderPart("right"),
                BorderPart("top"),
                BorderPart("bottom"),
                new XElement(ns + "diagonal"));
        }

        private static XElement CreateXf(
            XNamespace ns,
            int fontId = 0,
            int fillId = 0,
            int borderId = 0,
            int numFmtId = 0,
            string? alignment = null,
            string? verticalAlignment = null)
        {
            var attributes = new List<XAttribute>
            {
                new("numFmtId", numFmtId),
                new("fontId", fontId),
                new("fillId", fillId),
                new("borderId", borderId),
                new("xfId", "0")
            };

            if (numFmtId > 0)
            {
                attributes.Add(new XAttribute("applyNumberFormat", "1"));
            }

            if (fontId > 0)
            {
                attributes.Add(new XAttribute("applyFont", "1"));
            }

            if (fillId > 0)
            {
                attributes.Add(new XAttribute("applyFill", "1"));
            }

            if (borderId > 0)
            {
                attributes.Add(new XAttribute("applyBorder", "1"));
            }

            var elements = new List<XElement>();
            if (alignment != null || verticalAlignment != null)
            {
                attributes.Add(new XAttribute("applyAlignment", "1"));
                var alignmentAttributes = new List<XAttribute>();

                if (alignment != null)
                {
                    alignmentAttributes.Add(new XAttribute("horizontal", alignment));
                }

                if (verticalAlignment != null)
                {
                    alignmentAttributes.Add(new XAttribute("vertical", verticalAlignment));
                }

                elements.Add(new XElement(ns + "alignment", alignmentAttributes));
            }

            return new XElement(ns + "xf", attributes, elements);
        }

        private static XDocument CreatePumpOperationsSheetXml(IReadOnlyList<PumpOperationDto> operations)
        {
            XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

            var totalConcrete = operations.Sum(x => x.ConcreteVolume);
            var totalAdditionalStanding = operations.Sum(x => x.AdditionalStanding);
            var totalDismantleAssemble = operations.Sum(x => x.DismantleAssemble);
            var totalIdleCount = operations.Count(x => x.IsIdle);
            var totalDailyCount = operations.Count(x => x.IsDaily);

            var rows = new List<XElement>();
            var row = 1;

            rows.Add(CreateRow(ns, row++, ExportHeaders.Select(header => Cell(header, HeaderStyle)), 28));

            for (var index = 0; index < operations.Count; index++)
            {
                var operation = operations[index];
                rows.Add(CreateRow(ns, row++, new[]
                {
                    Cell(index + 1, IntegerStyle),
                    Cell(operation.Date.Date, DateStyle),
                    Cell(operation.ReviewStatus, TextStyle),
                    Cell($"{operation.OperatorFirstName} {operation.OperatorLastName}".Trim(), TextStyle),
                    Cell(operation.CompanyName, TextStyle),
                    Cell(operation.AddressName, TextStyle),
                    Cell(operation.PlateNumber, TextStyle),
                    Cell(GetDistanceRangeLabel(operation), TextStyle),
                    Cell(operation.ConcreteVolume, DecimalStyle),
                    Cell(operation.AdditionalStanding, IntegerStyle),
                    Cell(operation.DismantleAssemble, IntegerStyle),
                    Cell(operation.IsIdle ? "კი" : "არა", TextStyle),
                    Cell(operation.IsDaily ? "კი" : "არა", TextStyle),
                    Cell(operation.Note ?? "", TextStyle)
                }, 24));
            }

            if (operations.Count == 0)
            {
                rows.Add(CreateRow(ns, row++, new[]
                {
                    Cell("", IntegerStyle),
                    Cell("", DateStyle),
                    Cell("ჩანაწერები არ მოიძებნა", TextStyle),
                    Cell("", TextStyle),
                    Cell("", TextStyle),
                    Cell("", TextStyle),
                    Cell("", TextStyle),
                    Cell("", TextStyle),
                    Cell(0, DecimalStyle),
                    Cell(0, IntegerStyle),
                    Cell(0, IntegerStyle),
                    Cell("არა", TextStyle),
                    Cell("არა", TextStyle),
                    Cell("", TextStyle)
                }, 24));
            }

            rows.Add(CreateRow(ns, row, new[]
            {
                Cell($"სულ ჩანაწერები: {operations.Count}", TotalTextStyle),
                Cell("", TotalTextStyle),
                Cell("", TotalTextStyle),
                Cell("", TotalTextStyle),
                Cell("", TotalTextStyle),
                Cell("", TotalTextStyle),
                Cell("", TotalTextStyle),
                Cell("", TotalTextStyle),
                Cell(totalConcrete, TotalDecimalStyle),
                Cell(totalAdditionalStanding, TotalIntegerStyle),
                Cell(totalDismantleAssemble, TotalIntegerStyle),
                Cell(totalIdleCount, TotalIntegerStyle),
                Cell(totalDailyCount, TotalIntegerStyle),
                Cell("", TotalTextStyle)
            }, 26));

            return new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "worksheet",
                    new XElement(ns + "dimension", new XAttribute("ref", $"A1:N{GetExportEndRow(operations)}")),
                    CreateSheetViews(ns),
                    CreateColumns(ns),
                    new XElement(ns + "sheetData", rows),
                    new XElement(ns + "autoFilter", new XAttribute("ref", $"A1:N{GetDataEndRow(operations)}")),
                    new XElement(ns + "pageMargins",
                        new XAttribute("left", "0.7"),
                        new XAttribute("right", "0.7"),
                        new XAttribute("top", "0.75"),
                        new XAttribute("bottom", "0.75"),
                        new XAttribute("header", "0.3"),
                        new XAttribute("footer", "0.3"))));
        }

        private static XElement CreateSheetViews(XNamespace ns)
        {
            return new XElement(ns + "sheetViews",
                new XElement(ns + "sheetView",
                    new XAttribute("workbookViewId", "0"),
                    new XElement(ns + "pane",
                        new XAttribute("ySplit", "1"),
                        new XAttribute("topLeftCell", "A2"),
                        new XAttribute("activePane", "bottomLeft"),
                        new XAttribute("state", "frozen")),
                    new XElement(ns + "selection",
                        new XAttribute("pane", "bottomLeft"),
                        new XAttribute("activeCell", "A2"),
                        new XAttribute("sqref", "A2"))));
        }

        private static int GetDataEndRow(IReadOnlyList<PumpOperationDto> operations)
        {
            return 1 + Math.Max(operations.Count, 1);
        }

        private static int GetExportEndRow(IReadOnlyList<PumpOperationDto> operations)
        {
            return GetDataEndRow(operations) + 1;
        }

        private static XElement CreateColumns(XNamespace ns)
        {
            var widths = new[] { 6, 13, 15, 24, 26, 30, 16, 22, 14, 20, 18, 14, 12, 40 };

            return new XElement(ns + "cols",
                widths.Select((width, index) =>
                    new XElement(ns + "col",
                        new XAttribute("min", index + 1),
                        new XAttribute("max", index + 1),
                        new XAttribute("width", width),
                        new XAttribute("customWidth", "1"))));
        }

        private static ExcelCell Cell(object? value, uint styleIndex = DefaultStyle)
        {
            return new ExcelCell(value, styleIndex);
        }

        private static string GetDistanceRangeLabel(PumpOperationDto operation)
        {
            if (!string.Equals(operation.PumpType, "<60მ", StringComparison.Ordinal))
            {
                return "—";
            }

            return operation.DistanceRange ?? "60მ-მდე";
        }

        private static XElement CreateRow(
            XNamespace ns,
            int rowIndex,
            IEnumerable<ExcelCell> cells,
            double? height = null)
        {
            var attributes = new List<XAttribute>
            {
                new("r", rowIndex)
            };

            if (height.HasValue)
            {
                attributes.Add(new XAttribute("ht", height.Value.ToString(CultureInfo.InvariantCulture)));
                attributes.Add(new XAttribute("customHeight", "1"));
            }

            return new XElement(ns + "row",
                attributes,
                cells.Select((cell, index) => CreateCell(ns, rowIndex, index + 1, cell)));
        }

        private static XElement CreateCell(XNamespace ns, int rowIndex, int columnIndex, ExcelCell cell)
        {
            var reference = $"{GetColumnName(columnIndex)}{rowIndex}";
            var value = cell.Value;

            return value switch
            {
                DateTime dateValue => CreateNumberCell(ns, reference, dateValue.ToOADate().ToString(CultureInfo.InvariantCulture), cell.StyleIndex),
                decimal decimalValue => CreateNumberCell(ns, reference, decimalValue.ToString(CultureInfo.InvariantCulture), cell.StyleIndex),
                int intValue => CreateNumberCell(ns, reference, intValue.ToString(CultureInfo.InvariantCulture), cell.StyleIndex),
                long longValue => CreateNumberCell(ns, reference, longValue.ToString(CultureInfo.InvariantCulture), cell.StyleIndex),
                double doubleValue => CreateNumberCell(ns, reference, doubleValue.ToString(CultureInfo.InvariantCulture), cell.StyleIndex),
                float floatValue => CreateNumberCell(ns, reference, floatValue.ToString(CultureInfo.InvariantCulture), cell.StyleIndex),
                _ => CreateTextCell(ns, reference, value?.ToString() ?? string.Empty, cell.StyleIndex)
            };
        }

        private static XElement CreateNumberCell(XNamespace ns, string reference, string value, uint styleIndex)
        {
            var attributes = CreateCellAttributes(reference, styleIndex);

            return new XElement(ns + "c",
                attributes,
                new XElement(ns + "v", value));
        }

        private static XElement CreateTextCell(XNamespace ns, string reference, string value, uint styleIndex)
        {
            var attributes = CreateCellAttributes(reference, styleIndex);
            attributes.Add(new XAttribute("t", "inlineStr"));

            return new XElement(ns + "c",
                attributes,
                new XElement(ns + "is",
                    new XElement(ns + "t",
                        string.IsNullOrWhiteSpace(value) ? null : new XAttribute(XNamespace.Xml + "space", "preserve"),
                        value)));
        }

        private static List<XAttribute> CreateCellAttributes(string reference, uint styleIndex)
        {
            var attributes = new List<XAttribute>
            {
                new("r", reference)
            };

            if (styleIndex != DefaultStyle)
            {
                attributes.Add(new XAttribute("s", styleIndex));
            }

            return attributes;
        }

        private static string GetColumnName(int columnIndex)
        {
            var dividend = columnIndex;
            var columnName = string.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }

        private static void AddZipEntry(ZipArchive archive, string path, XDocument document)
        {
            var entry = archive.CreateEntry(path);

            using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            document.Save(writer, SaveOptions.DisableFormatting);
        }
    }
}

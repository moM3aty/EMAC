using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMAC.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Service = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeSlot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProblemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technicians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoveredRegions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingHoursStart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingHoursEnd = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technicians", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: true),
                    DeviceModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhysicalCondition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accessories = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopTickets_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InitialReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkshopTicketId = table.Column<int>(type: "int", nullable: false),
                    FaultDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredSpareParts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicianNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerApproval = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitialReports_WorkshopTickets_WorkshopTicketId",
                        column: x => x.WorkshopTicketId,
                        principalTable: "WorkshopTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkshopTicketId = table.Column<int>(type: "int", nullable: false),
                    LaborCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SparePartsCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_WorkshopTickets_WorkshopTicketId",
                        column: x => x.WorkshopTicketId,
                        principalTable: "WorkshopTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ServiceCategories",
                columns: new[] { "Id", "ArabicName", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "صيانة مكيفات سبليت/شباك", "ac_maint", "غسيل وتنظيف وإصلاح أعطال", true },
                    { 2, "صيانة تكييف مركزي", "central_maint", "للمجمعات والفلل", true },
                    { 3, "تمديد مواسير نحاس", "pipe_extension", "تأسيس وتمديد", true },
                    { 4, "صيانة ثلاجات ومجمدات", "fridge_repair", "إصلاح منزلي وتجاري", true },
                    { 5, "صيانة غسالات", "wash_repair", "أوتوماتيك وعادي", true },
                    { 6, "صيانة أفران وميكروويف", "oven_repair", "كهرباء وغاز", true },
                    { 7, "تركيب/صيانة غرف تبريد", "fridge_room_install", "مخازن التبريد", true },
                    { 8, "صيانة تكييف سيارات مبردة", "car_ac", "شاحنات النقل المبرد", true }
                });

            migrationBuilder.InsertData(
                table: "ServiceRequests",
                columns: new[] { "Id", "AppointmentDate", "CreatedAt", "CustomerName", "DeviceCode", "Location", "PhoneNumber", "ProblemDescription", "RequestNumber", "ServiceType", "Status", "TimeSlot" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 28, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5203), new DateTime(2025, 11, 28, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5209), "عبدالله محمد", "DEV-AC-101", "hassa", "0501234567", "المكيف لا يبرد ويصدر صوت عالي", "REQ-20251120-1001", "ac_maint", "InWorkshop", "09:00 ص" },
                    { 2, new DateTime(2025, 11, 29, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5215), new DateTime(2025, 11, 29, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5218), "شركة النقل السريع", "DEV-CA-205", "dammam", "0559876543", "صيانة دورية لأسطول الشاحنات المبردة", "REQ-20251121-2005", "car_ac", "Confirmed", "02:00 م" },
                    { 3, new DateTime(2025, 11, 25, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5222), new DateTime(2025, 11, 25, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5224), "سارة أحمد", "DEV-FR-304", "riyadh", "0561112233", "الثلاجة لا تعمل نهائياً", "REQ-20251115-3040", "fridge_repair", "Closed", "10:00 ص" }
                });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "CoveredRegions", "FullName", "Specialty", "WorkingHoursEnd", "WorkingHoursStart" },
                values: new object[,]
                {
                    { 1, "hassa,dammam", "أحمد علي", "ac_maint", "18:00", "08:00" },
                    { 2, "riyadh,hassa", "محمد سامي", "fridge_repair", "18:00", "08:00" },
                    { 3, "all", "فريق المشاريع (أ)", "central_maint", "18:00", "08:00" },
                    { 4, "hassa,dammam,riyadh,other,all", "فني عام (للتجربة)", "General", "18:00", "08:00" }
                });

            migrationBuilder.InsertData(
                table: "WorkshopTickets",
                columns: new[] { "Id", "Accessories", "DeviceCode", "DeviceModel", "PhysicalCondition", "ReceivedAt", "SerialNumber", "ServiceRequestId", "Status" },
                values: new object[,]
                {
                    { 1, "ريموت كنترول", "WS-1122-5544", "مكيف سبليت جنرال 24 وحدة", "خدوش بسيطة في الغطاء الخارجي", new DateTime(2025, 11, 28, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5258), "SN-99887766", 1, "Inspected" },
                    { 2, "بدون", "WS-1115-3322", "ثلاجة سامسونج 20 قدم", "سليمة", new DateTime(2025, 11, 25, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5264), "SN-11223344", 3, "Closed" }
                });

            migrationBuilder.InsertData(
                table: "InitialReports",
                columns: new[] { "Id", "CreatedAt", "CustomerApproval", "EstimatedCost", "EstimatedTime", "FaultDescription", "RequiredSpareParts", "TechnicianNotes", "WorkshopTicketId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 29, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5294), false, 850m, "3 أيام", "تلف في الكومبريسور ونقص فريون", "كومبريسور جديد، غاز فريون", "يحتاج تنظيف شامل للوحدة الداخلية", 1 },
                    { 2, new DateTime(2025, 11, 26, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5299), true, 350m, "يوم واحد", "عطل في الثرموستات", "ثرموستات أصلي", "تم الاختبار بنجاح", 2 }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "Id", "IsPaid", "IssuedAt", "LaborCost", "PaymentMethod", "SparePartsCost", "TaxAmount", "TotalAmount", "WorkshopTicketId" },
                values: new object[] { 1, true, new DateTime(2025, 11, 27, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5336), 150m, "CreditCard", 200m, 52.5m, 402.5m, 2 });

            migrationBuilder.CreateIndex(
                name: "IX_InitialReports_WorkshopTicketId",
                table: "InitialReports",
                column: "WorkshopTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_WorkshopTicketId",
                table: "Invoices",
                column: "WorkshopTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopTickets_ServiceRequestId",
                table: "WorkshopTickets",
                column: "ServiceRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "InitialReports");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "ServiceCategories");

            migrationBuilder.DropTable(
                name: "Technicians");

            migrationBuilder.DropTable(
                name: "WorkshopTickets");

            migrationBuilder.DropTable(
                name: "ServiceRequests");
        }
    }
}

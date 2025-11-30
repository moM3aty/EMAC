using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMAC.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TechnicianId",
                table: "ServiceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "InitialReports",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(5753));

            migrationBuilder.UpdateData(
                table: "InitialReports",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(5761));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 1,
                column: "IssuedAt",
                value: new DateTime(2025, 11, 27, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(5823));

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AppointmentDate", "CreatedAt", "TechnicianId" },
                values: new object[] { new DateTime(2025, 11, 28, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(4081), new DateTime(2025, 11, 28, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(4087), null });

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AppointmentDate", "CreatedAt", "TechnicianId" },
                values: new object[] { new DateTime(2025, 11, 29, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(4093), new DateTime(2025, 11, 29, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(4096), null });

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AppointmentDate", "CreatedAt", "TechnicianId" },
                values: new object[] { new DateTime(2025, 11, 25, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(4892), new DateTime(2025, 11, 25, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(4895), null });

            migrationBuilder.UpdateData(
                table: "WorkshopTickets",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReceivedAt",
                value: new DateTime(2025, 11, 28, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(5565));

            migrationBuilder.UpdateData(
                table: "WorkshopTickets",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReceivedAt",
                value: new DateTime(2025, 11, 25, 14, 36, 51, 471, DateTimeKind.Local).AddTicks(5581));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_TechnicianId",
                table: "ServiceRequests",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Technicians_TechnicianId",
                table: "ServiceRequests",
                column: "TechnicianId",
                principalTable: "Technicians",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Technicians_TechnicianId",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_TechnicianId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "ServiceRequests");

            migrationBuilder.UpdateData(
                table: "InitialReports",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4454));

            migrationBuilder.UpdateData(
                table: "InitialReports",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4460));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 1,
                column: "IssuedAt",
                value: new DateTime(2025, 11, 27, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4494));

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 28, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4360), new DateTime(2025, 11, 28, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4365) });

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 29, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4370), new DateTime(2025, 11, 29, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4373) });

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 25, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4377), new DateTime(2025, 11, 25, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4379) });

            migrationBuilder.UpdateData(
                table: "WorkshopTickets",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReceivedAt",
                value: new DateTime(2025, 11, 28, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4418));

            migrationBuilder.UpdateData(
                table: "WorkshopTickets",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReceivedAt",
                value: new DateTime(2025, 11, 25, 14, 23, 27, 94, DateTimeKind.Local).AddTicks(4423));
        }
    }
}

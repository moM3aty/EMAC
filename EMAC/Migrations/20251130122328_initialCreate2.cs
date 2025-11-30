using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMAC.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RequestNumber",
                table: "ServiceRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ProblemDescription",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceCode",
                table: "ServiceRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestNumber",
                table: "ServiceRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProblemDescription",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceCode",
                table: "ServiceRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "InitialReports",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5294));

            migrationBuilder.UpdateData(
                table: "InitialReports",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5299));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 1,
                column: "IssuedAt",
                value: new DateTime(2025, 11, 27, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5336));

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 28, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5203), new DateTime(2025, 11, 28, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5209) });

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 29, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5215), new DateTime(2025, 11, 29, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5218) });

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AppointmentDate", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 25, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5222), new DateTime(2025, 11, 25, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5224) });

            migrationBuilder.UpdateData(
                table: "WorkshopTickets",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReceivedAt",
                value: new DateTime(2025, 11, 28, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5258));

            migrationBuilder.UpdateData(
                table: "WorkshopTickets",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReceivedAt",
                value: new DateTime(2025, 11, 25, 14, 19, 51, 577, DateTimeKind.Local).AddTicks(5264));
        }
    }
}

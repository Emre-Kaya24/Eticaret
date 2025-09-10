using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class addorderAndOrderLineRelation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 9, 10, 1, 4, 34, 32, DateTimeKind.Local).AddTicks(7363), new Guid("4a7c7f5a-e75d-4e60-ac95-fc7ab4be35fe") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 1, 4, 34, 35, DateTimeKind.Local).AddTicks(543));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 1, 4, 34, 35, DateTimeKind.Local).AddTicks(1480));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 9, 10, 1, 0, 27, 37, DateTimeKind.Local).AddTicks(4034), new Guid("823dbfee-7147-41b0-b11f-34908b5bd2f0") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 1, 0, 27, 39, DateTimeKind.Local).AddTicks(7183));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 1, 0, 27, 39, DateTimeKind.Local).AddTicks(8117));
        }
    }
}

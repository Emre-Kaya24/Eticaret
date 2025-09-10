using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAndOrderLineRelation3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 9, 10, 1, 6, 29, 657, DateTimeKind.Local).AddTicks(6679), new Guid("e0160fb4-fe78-421f-adf4-afe98a46443a") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 1, 6, 29, 660, DateTimeKind.Local).AddTicks(1542));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 9, 10, 1, 6, 29, 660, DateTimeKind.Local).AddTicks(2493));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}

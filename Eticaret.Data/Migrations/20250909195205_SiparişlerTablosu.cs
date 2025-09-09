using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eticaret.Data.Migrations
{
    /// <inheritdoc />
    public partial class SiparişlerTablosu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AppUserId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BillingAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLines_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 9, 9, 22, 52, 4, 299, DateTimeKind.Local).AddTicks(4506), new Guid("7f97cdb3-13a5-4674-97c4-577f769ee5be") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 9, 9, 22, 52, 4, 301, DateTimeKind.Local).AddTicks(9762));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 9, 9, 22, 52, 4, 302, DateTimeKind.Local).AddTicks(667));

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_OrderId",
                table: "OrderLines",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_ProductId",
                table: "OrderLines",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderLines");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 9, 1, 23, 21, 56, 813, DateTimeKind.Local).AddTicks(4596), new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateDate",
                value: new DateTime(2025, 9, 1, 23, 21, 56, 816, DateTimeKind.Local).AddTicks(1772));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateDate",
                value: new DateTime(2025, 9, 1, 23, 21, 56, 816, DateTimeKind.Local).AddTicks(2598));
        }
    }
}

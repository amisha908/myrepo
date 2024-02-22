using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecom.Services.AuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class auth2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3fb73f8f-f6d8-4c1d-a426-ed5e450a97ce");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "adc3d5d2-1932-44ae-b112-3c386a623b7f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "26e236fa-6e32-4c71-ae48-014199e55df8", "1", "Admin", "Admin" },
                    { "f1d477bb-9788-43df-9090-50afe0bc130a", "2", "User", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26e236fa-6e32-4c71-ae48-014199e55df8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f1d477bb-9788-43df-9090-50afe0bc130a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3fb73f8f-f6d8-4c1d-a426-ed5e450a97ce", "2", "User", "User" },
                    { "adc3d5d2-1932-44ae-b112-3c386a623b7f", "1", "Admin", "Admin" }
                });
        }
    }
}

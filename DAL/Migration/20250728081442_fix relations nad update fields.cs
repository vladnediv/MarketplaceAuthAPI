using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migration
{
    /// <inheritdoc />
    public partial class fixrelationsnadupdatefields : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_ShopId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "OrderIds",
                table: "MarketplaceUsers");

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                table: "MarketplaceShops",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ShopId",
                table: "Addresses",
                column: "ShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_ShopId",
                table: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "OrderIds",
                table: "MarketplaceUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                table: "MarketplaceShops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ShopId",
                table: "Addresses",
                column: "ShopId",
                unique: true,
                filter: "[ShopId] IS NOT NULL");
        }
    }
}

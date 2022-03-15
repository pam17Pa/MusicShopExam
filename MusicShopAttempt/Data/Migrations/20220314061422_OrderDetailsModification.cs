using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicShopAttempt.Data.Migrations
{
    public partial class OrderDetailsModification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "OrderDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderDetails");
        }
    }
}

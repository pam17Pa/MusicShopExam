using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicShopAttempt.Data.Migrations
{
    public partial class Pic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "Products",
                newName: "PictureName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PictureName",
                table: "Products",
                newName: "Picture");
        }
    }
}

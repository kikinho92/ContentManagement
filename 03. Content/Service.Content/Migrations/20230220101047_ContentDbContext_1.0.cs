using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Content.Migrations
{
    public partial class ContentDbContext_10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Content",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Grades",
                table: "Content",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Grades",
                table: "Content");
        }
    }
}

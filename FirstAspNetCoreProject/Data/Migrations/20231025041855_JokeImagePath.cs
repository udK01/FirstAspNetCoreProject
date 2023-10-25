using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstAspNetCoreProject.Data.Migrations
{
    public partial class JokeImagePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JokeImagePath",
                table: "Joke",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JokeImagePath",
                table: "Joke");
        }
    }
}

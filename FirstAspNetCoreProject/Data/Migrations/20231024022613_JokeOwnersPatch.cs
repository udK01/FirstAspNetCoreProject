using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstAspNetCoreProject.Data.Migrations
{
    public partial class JokeOwnersPatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JokeOwner",
                table: "Joke",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JokeOwner",
                table: "Joke");
        }
    }
}

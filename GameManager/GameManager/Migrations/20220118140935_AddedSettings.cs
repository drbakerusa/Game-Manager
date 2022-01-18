using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameManager.Migrations
{
    public partial class AddedSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    F95Username = table.Column<string>(type: "TEXT", nullable: false),
                    F95Password = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultPageSize = table.Column<int>(type: "INTEGER", nullable: false),
                    AutomaticallyCheckForGameUpdates = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}

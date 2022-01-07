using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameManager.Migrations
{
    public partial class RemovedThreadId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThreadId",
                table: "GameMetadata");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThreadId",
                table: "GameMetadata",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameManager.Migrations
{
    public partial class AddedRecentThresholdDays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecentThresholdDays",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecentThresholdDays",
                table: "Settings");
        }
    }
}

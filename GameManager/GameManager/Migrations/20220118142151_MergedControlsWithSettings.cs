using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameManager.Migrations
{
    public partial class MergedControlsWithSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LatestApplicationVersion",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MetadataUpdateControlId",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "NewerVersionExists",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestApplicationVersion",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "MetadataUpdateControlId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "NewerVersionExists",
                table: "Settings");
        }
    }
}

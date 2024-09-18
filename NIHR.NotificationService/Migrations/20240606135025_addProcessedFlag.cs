using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NIHR.NotificationService.Migrations
{
    public partial class addProcessedFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "Notifications",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "Notifications");
        }
    }
}

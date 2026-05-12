using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdoptMeNow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRescueReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusImagePath",
                table: "RescueReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusNote",
                table: "RescueReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusUpdatedAt",
                table: "RescueReports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusImagePath",
                table: "RescueReports");

            migrationBuilder.DropColumn(
                name: "StatusNote",
                table: "RescueReports");

            migrationBuilder.DropColumn(
                name: "StatusUpdatedAt",
                table: "RescueReports");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdoptMeNow.Migrations
{
    /// <inheritdoc />
    public partial class AddRescueProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "RescueProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RescueReportId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RescueProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RescueProgresses_RescueReports_RescueReportId",
                        column: x => x.RescueReportId,
                        principalTable: "RescueReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RescueProgresses_RescueReportId",
                table: "RescueProgresses",
                column: "RescueReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RescueProgresses");

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
    }
}

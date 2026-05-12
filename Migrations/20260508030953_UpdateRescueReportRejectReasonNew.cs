using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdoptMeNow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRescueReportRejectReasonNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "RescueReports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "RescueReports");
        }
    }
}

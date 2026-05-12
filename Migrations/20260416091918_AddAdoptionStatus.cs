using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdoptMeNow.Migrations
{
    /// <inheritdoc />
    public partial class AddAdoptionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Adoptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Adoptions");
        }
    }
}

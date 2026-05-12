using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdoptMeNow.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsAdopted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdopted",
                table: "Pets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdopted",
                table: "Pets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

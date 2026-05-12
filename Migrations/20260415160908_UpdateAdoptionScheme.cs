using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdoptMeNow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdoptionScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HousingType",
                table: "Adoptions");

            migrationBuilder.DropColumn(
                name: "OwnershipStatus",
                table: "Adoptions");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Adoptions");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "Adoptions");

            migrationBuilder.RenameColumn(
                name: "HasPetExperience",
                table: "Adoptions",
                newName: "WillingVetVisits");

            migrationBuilder.RenameColumn(
                name: "HasOtherPets",
                table: "Adoptions",
                newName: "IsFinanciallyReady");

            migrationBuilder.AddColumn<bool>(
                name: "AgreeTerms",
                table: "Adoptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasEnoughSpace",
                table: "Adoptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasTimeForPet",
                table: "Adoptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmotionallyReady",
                table: "Adoptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreeTerms",
                table: "Adoptions");

            migrationBuilder.DropColumn(
                name: "HasEnoughSpace",
                table: "Adoptions");

            migrationBuilder.DropColumn(
                name: "HasTimeForPet",
                table: "Adoptions");

            migrationBuilder.DropColumn(
                name: "IsEmotionallyReady",
                table: "Adoptions");

            migrationBuilder.RenameColumn(
                name: "WillingVetVisits",
                table: "Adoptions",
                newName: "HasPetExperience");

            migrationBuilder.RenameColumn(
                name: "IsFinanciallyReady",
                table: "Adoptions",
                newName: "HasOtherPets");

            migrationBuilder.AddColumn<string>(
                name: "HousingType",
                table: "Adoptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnershipStatus",
                table: "Adoptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Adoptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkingHours",
                table: "Adoptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

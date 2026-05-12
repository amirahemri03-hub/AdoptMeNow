using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdoptMeNow.Migrations
{
    /// <inheritdoc />
    public partial class AddPetNavigationToAdoption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Adoptions_PetId",
                table: "Adoptions",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Adoptions_Pets_PetId",
                table: "Adoptions",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adoptions_Pets_PetId",
                table: "Adoptions");

            migrationBuilder.DropIndex(
                name: "IX_Adoptions_PetId",
                table: "Adoptions");
        }
    }
}

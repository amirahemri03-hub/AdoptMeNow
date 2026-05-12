using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdoptMeNow.Migrations
{
    /// <inheritdoc />
    public partial class AddAdoptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adoptions",
                columns: table => new
                {
                    AdoptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IcNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HousingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnershipStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasPetExperience = table.Column<bool>(type: "bit", nullable: false),
                    HasOtherPets = table.Column<bool>(type: "bit", nullable: false),
                    WorkingHours = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adoptions", x => x.AdoptionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adoptions");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCarMakesAndModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarMakes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarMakes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MakeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarModels_CarMakes_MakeId",
                        column: x => x.MakeId,
                        principalTable: "CarMakes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CarMakes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Toyota" },
                    { 2, "Honda" },
                    { 3, "Ford" },
                    { 4, "Chevrolet" },
                    { 5, "BMW" },
                    { 6, "Mercedes-Benz" },
                    { 7, "Audi" },
                    { 8, "Volkswagen" },
                    { 9, "Nissan" },
                    { 10, "Hyundai" }
                });

            migrationBuilder.InsertData(
                table: "CarModels",
                columns: new[] { "Id", "MakeId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Camry" },
                    { 2, 1, "Corolla" },
                    { 3, 1, "RAV4" },
                    { 4, 1, "Highlander" },
                    { 5, 1, "Prius" },
                    { 6, 1, "Tacoma" },
                    { 7, 1, "4Runner" },
                    { 8, 2, "Civic" },
                    { 9, 2, "Accord" },
                    { 10, 2, "CR-V" },
                    { 11, 2, "Pilot" },
                    { 12, 2, "Odyssey" },
                    { 13, 2, "HR-V" },
                    { 14, 3, "F-150" },
                    { 15, 3, "Mustang" },
                    { 16, 3, "Explorer" },
                    { 17, 3, "Escape" },
                    { 18, 3, "Edge" },
                    { 19, 3, "Bronco" },
                    { 20, 4, "Silverado" },
                    { 21, 4, "Equinox" },
                    { 22, 4, "Malibu" },
                    { 23, 4, "Traverse" },
                    { 24, 4, "Tahoe" },
                    { 25, 4, "Camaro" },
                    { 26, 5, "3 Series" },
                    { 27, 5, "5 Series" },
                    { 28, 5, "X3" },
                    { 29, 5, "X5" },
                    { 30, 5, "X7" },
                    { 31, 5, "M3" },
                    { 32, 6, "C-Class" },
                    { 33, 6, "E-Class" },
                    { 34, 6, "GLC" },
                    { 35, 6, "GLE" },
                    { 36, 6, "S-Class" },
                    { 37, 6, "GLA" },
                    { 38, 7, "A4" },
                    { 39, 7, "A6" },
                    { 40, 7, "Q5" },
                    { 41, 7, "Q7" },
                    { 42, 7, "A3" },
                    { 43, 7, "Q3" },
                    { 44, 8, "Jetta" },
                    { 45, 8, "Passat" },
                    { 46, 8, "Tiguan" },
                    { 47, 8, "Atlas" },
                    { 48, 8, "Golf" },
                    { 49, 8, "Taos" },
                    { 50, 9, "Altima" },
                    { 51, 9, "Sentra" },
                    { 52, 9, "Rogue" },
                    { 53, 9, "Pathfinder" },
                    { 54, 9, "Murano" },
                    { 55, 9, "Frontier" },
                    { 56, 10, "Elantra" },
                    { 57, 10, "Sonata" },
                    { 58, 10, "Tucson" },
                    { 59, 10, "Santa Fe" },
                    { 60, 10, "Palisade" },
                    { 61, 10, "Kona" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarModels_MakeId",
                table: "CarModels",
                column: "MakeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarModels");

            migrationBuilder.DropTable(
                name: "CarMakes");
        }
    }
}

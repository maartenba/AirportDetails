using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportDetails.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", nullable: false),
                    IsoCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    NormalizedIsoCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", nullable: false),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NormalizedCity = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Iata = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    NormalizedIata = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,5)", nullable: false),
                    Timezone = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Airports_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Airports_City",
                table: "Airports",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_CountryId",
                table: "Airports",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_Iata",
                table: "Airports",
                column: "Iata");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_NormalizedCity",
                table: "Airports",
                column: "NormalizedCity");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_NormalizedIata",
                table: "Airports",
                column: "NormalizedIata");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_IsoCode",
                table: "Countries",
                column: "IsoCode");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_NormalizedIsoCode",
                table: "Countries",
                column: "NormalizedIsoCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Airports");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}

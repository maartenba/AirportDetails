using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportDetails.Migrations
{
    /// <inheritdoc />
    public partial class Airlines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Airlines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", nullable: false),
                    Iata = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    NormalizedIata = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Callsign = table.Column<string>(type: "TEXT", nullable: false),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airlines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Airlines_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_CountryId",
                table: "Airlines",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_Iata",
                table: "Airlines",
                column: "Iata");

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_NormalizedIata",
                table: "Airlines",
                column: "NormalizedIata");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Airlines");
        }
    }
}

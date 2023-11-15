using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportDetails.Migrations
{
    /// <inheritdoc />
    public partial class AirlinesAirports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirlineAirport",
                columns: table => new
                {
                    AirlinesId = table.Column<int>(type: "INTEGER", nullable: false),
                    AirportsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirlineAirport", x => new { x.AirlinesId, x.AirportsId });
                    table.ForeignKey(
                        name: "FK_AirlineAirport_Airlines_AirlinesId",
                        column: x => x.AirlinesId,
                        principalTable: "Airlines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AirlineAirport_Airports_AirportsId",
                        column: x => x.AirportsId,
                        principalTable: "Airports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirlineAirport_AirportsId",
                table: "AirlineAirport",
                column: "AirportsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirlineAirport");
        }
    }
}

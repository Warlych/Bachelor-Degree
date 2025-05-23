using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trains",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_identifier_value = table.Column<string>(type: "text", nullable: false),
                    parameters_gross_weight = table.Column<double>(type: "double precision", nullable: false),
                    parameters_length = table.Column<double>(type: "double precision", nullable: false),
                    parameters_net_weight = table.Column<double>(type: "double precision", nullable: false),
                    parameters_number_of_wagons = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trains", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trains");
        }
    }
}

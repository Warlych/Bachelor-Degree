using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movements.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "train_operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    railway_section_from_identifier_value = table.Column<string>(type: "text", nullable: false),
                    railway_section_to_identifier_value = table.Column<string>(type: "text", nullable: false),
                    train_identifier_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_train_operations", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "train_operations");
        }
    }
}

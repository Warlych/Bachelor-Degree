using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metrics.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "railway_sections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_identifier_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_railway_sections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "trains",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_identifier_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trains", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "metrics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_range_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_range_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    metrics_average_gross_weight = table.Column<double>(type: "double precision", nullable: false),
                    metrics_average_length = table.Column<double>(type: "double precision", nullable: false),
                    metrics_average_net_weight = table.Column<double>(type: "double precision", nullable: false),
                    metrics_route_speed = table.Column<double>(type: "double precision", nullable: false),
                    metrics_section_speed = table.Column<double>(type: "double precision", nullable: false),
                    metrics_technical_speed = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_metrics", x => x.id);
                    table.ForeignKey(
                        name: "fk_metrics_railway_sections_from_id",
                        column: x => x.from_id,
                        principalTable: "railway_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_metrics_railway_sections_metric_id",
                        column: x => x.metric_id,
                        principalTable: "railway_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetricTrains",
                columns: table => new
                {
                    metric_id = table.Column<Guid>(type: "uuid", nullable: false),
                    _trains_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_metric_trains", x => new { x.metric_id, x._trains_id });
                    table.ForeignKey(
                        name: "fk_metric_trains_metrics_metric_id",
                        column: x => x.metric_id,
                        principalTable: "metrics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_metric_trains_trains__trains_id",
                        column: x => x._trains_id,
                        principalTable: "trains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_metrics_from_id",
                table: "metrics",
                column: "from_id");

            migrationBuilder.CreateIndex(
                name: "ix_metrics_metric_id",
                table: "metrics",
                column: "metric_id");

            migrationBuilder.CreateIndex(
                name: "ix_metric_trains__trains_id",
                table: "MetricTrains",
                column: "_trains_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetricTrains");

            migrationBuilder.DropTable(
                name: "metrics");

            migrationBuilder.DropTable(
                name: "trains");

            migrationBuilder.DropTable(
                name: "railway_sections");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelemetryIngestionCore.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelemetryReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DeviceId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Battery = table.Column<int>(type: "INTEGER", nullable: false),
                    Signal = table.Column<int>(type: "INTEGER", nullable: false),
                    RecordedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryReadings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryReadings_DeviceId",
                table: "TelemetryReadings",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryReadings_RecordedAt",
                table: "TelemetryReadings",
                column: "RecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryReadings_TenantId",
                table: "TelemetryReadings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryReadings_TenantId_ExternalId ",
                table: "TelemetryReadings",
                columns: new[] { "TenantId", "ExternalId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelemetryReadings");
        }
    }
}

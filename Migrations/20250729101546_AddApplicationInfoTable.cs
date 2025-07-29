using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationPrime.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationInfoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    BusinessImpact = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TimeIssueStarted = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    TimeReported = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    ImpactedUsers = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationAffected = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LocationsAffected = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Workaround = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IncidentNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Urgency = table.Column<int>(type: "INTEGER", nullable: false),
                    IncidentType = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_IsActive",
                table: "Applications",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Name",
                table: "Applications",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Incidents");
        }
    }
}

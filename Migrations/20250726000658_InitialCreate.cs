using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationPrime.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogEntries",
                columns: table => new
                {
                    AuditId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IncidentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    User = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Field = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OldValue = table.Column<string>(type: "TEXT", nullable: true),
                    NewValue = table.Column<string>(type: "TEXT", nullable: true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogEntries", x => x.AuditId);
                });

            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IncidentNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Urgency = table.Column<int>(type: "INTEGER", nullable: false),
                    Impact = table.Column<int>(type: "INTEGER", nullable: false),
                    IncidentDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AffectedApplication = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SupportTeam = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NotificationEmails = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsNoiSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoiSentDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Resolution = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ResolvedDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClosedDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    IncidentCommander = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BusinessImpact = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AffectedUsersCount = table.Column<int>(type: "INTEGER", nullable: true),
                    IsCustomerFacing = table.Column<bool>(type: "INTEGER", nullable: true),
                    BridgeDetails = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CurrentStatusUpdate = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    LastStatusUpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RootCause = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    LessonsLearned = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IsPirRequired = table.Column<bool>(type: "INTEGER", nullable: true),
                    PirScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsPirCompleted = table.Column<bool>(type: "INTEGER", nullable: true),
                    EscalatedFromPreIncidentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EstimatedResolutionTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DowntimeMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    IdentifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PotentialImpact = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsEscalated = table.Column<bool>(type: "INTEGER", nullable: true),
                    EscalatedToIncidentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EscalatedDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PreventiveActions = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    RiskAssessment = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_IncidentId_Timestamp",
                table: "AuditLogEntries",
                columns: new[] { "IncidentId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogEntries");

            migrationBuilder.DropTable(
                name: "Incidents");
        }
    }
}

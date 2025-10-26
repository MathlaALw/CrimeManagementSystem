using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crime_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    ClearanceLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AreaCity = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    CaseType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    AuthorizationLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cases_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrimeReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaCity = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ReportDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReportedByUserId = table.Column<int>(type: "int", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrimeReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrimeReports_Users_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CaseAssignees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedRole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProgressStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseAssignees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseAssignees_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseAssignees_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    AddedByUserId = table.Column<int>(type: "int", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParticipantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseParticipants_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseParticipants_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseParticipants_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Evidences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TextContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    AddedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidences", x => x.Id);
                    table.CheckConstraint("CK_Evidence_TextOrImage", "(Type = 0 AND TextContent IS NOT NULL AND FileUrl IS NULL) OR (Type = 1 AND FileUrl IS NOT NULL AND TextContent IS NULL)");
                    table.ForeignKey(
                        name: "FK_Evidences_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidences_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LinkedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseReports_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseReports_CrimeReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "CrimeReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvidenceAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActedByUserId = table.Column<int>(type: "int", nullable: false),
                    ActedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvidenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvidenceAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvidenceAuditLogs_Evidences_EvidenceId",
                        column: x => x.EvidenceId,
                        principalTable: "Evidences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvidenceAuditLogs_Users_ActedByUserId",
                        column: x => x.ActedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseAssignees_CaseId_UserId",
                table: "CaseAssignees",
                columns: new[] { "CaseId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseAssignees_UserId",
                table: "CaseAssignees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseParticipants_AddedByUserId",
                table: "CaseParticipants",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseParticipants_CaseId",
                table: "CaseParticipants",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseParticipants_ParticipantId",
                table: "CaseParticipants",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReports_CaseId_ReportId",
                table: "CaseReports",
                columns: new[] { "CaseId", "ReportId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseReports_ReportId",
                table: "CaseReports",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CaseNumber",
                table: "Cases",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CreatedByUserId",
                table: "Cases",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CrimeReports_ReportedByUserId",
                table: "CrimeReports",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidenceAuditLogs_ActedByUserId",
                table: "EvidenceAuditLogs",
                column: "ActedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidenceAuditLogs_EvidenceId",
                table: "EvidenceAuditLogs",
                column: "EvidenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_AddedByUserId",
                table: "Evidences",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_CaseId",
                table: "Evidences",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseAssignees");

            migrationBuilder.DropTable(
                name: "CaseParticipants");

            migrationBuilder.DropTable(
                name: "CaseReports");

            migrationBuilder.DropTable(
                name: "EvidenceAuditLogs");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "CrimeReports");

            migrationBuilder.DropTable(
                name: "Evidences");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

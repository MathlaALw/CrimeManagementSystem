using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crime_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddCitizenSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CitizenSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReceiveNewCrimes = table.Column<bool>(type: "bit", nullable: false),
                    ReceiveCaseUpdates = table.Column<bool>(type: "bit", nullable: false),
                    ReceiveAlerts = table.Column<bool>(type: "bit", nullable: false),
                    SubscribedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitizenSubscriptions_Email",
                table: "CitizenSubscriptions",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitizenSubscriptions");
        }
    }
}

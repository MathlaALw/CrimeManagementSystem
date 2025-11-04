using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crime_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseAuthorizationLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorizationLevel",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizationLevel",
                table: "Cases");
        }
    }
}

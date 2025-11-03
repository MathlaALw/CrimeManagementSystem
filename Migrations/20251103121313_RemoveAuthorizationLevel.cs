using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crime_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuthorizationLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizationLevel",
                table: "Cases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorizationLevel",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

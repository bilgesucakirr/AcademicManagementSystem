using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Venue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizerEmailToVenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizerEmail",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizerEmail",
                table: "Venues");
        }
    }
}

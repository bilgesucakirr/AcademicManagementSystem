using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Venue.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewFormUrlToVenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewFormUrl",
                table: "Venues",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewFormUrl",
                table: "Venues");
        }
    }
}

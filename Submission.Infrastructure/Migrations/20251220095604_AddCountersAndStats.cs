using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Submission.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCountersAndStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "Submissions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewersAssignedCount",
                table: "Submissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewsCompletedCount",
                table: "Submissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VenueSubmissionCounters",
                columns: table => new
                {
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CurrentCount = table.Column<int>(type: "int", nullable: false),
                    VenueAcronym = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueSubmissionCounters", x => new { x.VenueId, x.Year });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ReferenceNumber",
                table: "Submissions",
                column: "ReferenceNumber",
                unique: true,
                filter: "[ReferenceNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VenueSubmissionCounters");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ReferenceNumber",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ReviewersAssignedCount",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ReviewsCompletedCount",
                table: "Submissions");

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}

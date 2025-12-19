using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Review.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReviewAssignments_SubmissionId_ReviewerUserId",
                table: "ReviewAssignments");

            migrationBuilder.AddColumn<string>(
                name: "DeclineReason",
                table: "ReviewAssignments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeclinedAt",
                table: "ReviewAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "ReviewAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAssignments_SubmissionId_ReviewerUserId",
                table: "ReviewAssignments",
                columns: new[] { "SubmissionId", "ReviewerUserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReviewAssignments_SubmissionId_ReviewerUserId",
                table: "ReviewAssignments");

            migrationBuilder.DropColumn(
                name: "DeclineReason",
                table: "ReviewAssignments");

            migrationBuilder.DropColumn(
                name: "DeclinedAt",
                table: "ReviewAssignments");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "ReviewAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewAssignments_SubmissionId_ReviewerUserId",
                table: "ReviewAssignments",
                columns: new[] { "SubmissionId", "ReviewerUserId" },
                unique: true);
        }
    }
}

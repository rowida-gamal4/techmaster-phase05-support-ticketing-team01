using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTriageAssignmentUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentId",
                table: "TicketAssignments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignmentId",
                table: "TicketAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddResolutionNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResolutionNotes",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolutionNotes",
                table: "Tickets");
        }
    }
}

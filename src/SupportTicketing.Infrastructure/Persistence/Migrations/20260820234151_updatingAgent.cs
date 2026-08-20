using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportTicketing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updatingAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AgentProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AgentProfiles");
        }
    }
}

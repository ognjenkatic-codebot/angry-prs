using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngryPullRequests.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeGithubUsernameRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "github_username",
                table: "contributor",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "github_username",
                table: "contributor",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}

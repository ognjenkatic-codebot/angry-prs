using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngryPullRequests.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryCharacteristics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "repository_characteristics",
                columns: table => new
                {
                    repository_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pull_request_name_regex = table.Column<string>(type: "text", nullable: true),
                    pull_request_name_capture_regex = table.Column<string>(type: "text", nullable: true),
                    release_tag_regex = table.Column<string>(type: "text", nullable: true),
                    in_progress_label = table.Column<string>(type: "text", nullable: true),
                    small_pr_change_count = table.Column<int>(type: "integer", nullable: false),
                    large_pr_change_count = table.Column<int>(type: "integer", nullable: false),
                    old_pr_age_in_days = table.Column<int>(type: "integer", nullable: false),
                    inactive_pr_age_in_days = table.Column<int>(type: "integer", nullable: false),
                    delete_heave_ratio = table.Column<float>(type: "real", nullable: false),
                    slack_notification_channel = table.Column<string>(type: "text", nullable: true),
                    slack_access_token = table.Column<string>(type: "text", nullable: true),
                    slack_api_token = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_repository_characteristics", x => x.repository_id);
                    table.ForeignKey(
                        name: "fk_repository_characteristics_repositories_repository_id",
                        column: x => x.repository_id,
                        principalTable: "repositories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "repository_characteristics");
        }
    }
}

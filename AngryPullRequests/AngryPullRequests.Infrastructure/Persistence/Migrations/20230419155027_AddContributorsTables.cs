using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AngryPullRequests.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContributorsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contributor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    github_username = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contributor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "repository_contributor",
                columns: table => new
                {
                    contributor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    repository_id = table.Column<Guid>(type: "uuid", nullable: false),
                    merged_pull_request_count = table.Column<int>(type: "integer", nullable: true),
                    reviewed_pull_request_count = table.Column<int>(type: "integer", nullable: true),
                    first_merge_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_merge_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    first_review_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_review_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_repository_contributor", x => new { x.repository_id, x.contributor_id });
                    table.ForeignKey(
                        name: "fk_repository_contributor_contributor_contributor_id",
                        column: x => x.contributor_id,
                        principalTable: "contributor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_repository_contributor_repositories_repository_id",
                        column: x => x.repository_id,
                        principalTable: "repositories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_repository_contributor_contributor_id",
                table: "repository_contributor",
                column: "contributor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "repository_contributor");

            migrationBuilder.DropTable(
                name: "contributor");
        }
    }
}

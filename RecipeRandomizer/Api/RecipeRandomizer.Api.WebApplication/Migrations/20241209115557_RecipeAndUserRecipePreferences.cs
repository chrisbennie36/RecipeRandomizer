using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecipeRandomizer.Api.WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class RecipeAndUserRecipePreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeatPreferences");

            migrationBuilder.DropTable(
                name: "SeafoodPreferences");

            migrationBuilder.DropTable(
                name: "VegetarianPreferences");

            migrationBuilder.DropColumn(
                name: "PreferenceType",
                table: "RecipePreferences");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RecipePreferences",
                newName: "RecipeType");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RecipePreferences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserRecipePreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RecipePreferenceId = table.Column<int>(type: "integer", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRecipePreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRecipePreferences_RecipePreferences_RecipePreferenceId",
                        column: x => x.RecipePreferenceId,
                        principalTable: "RecipePreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRecipePreferences_RecipePreferenceId",
                table: "UserRecipePreferences",
                column: "RecipePreferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRecipePreferences");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RecipePreferences");

            migrationBuilder.RenameColumn(
                name: "RecipeType",
                table: "RecipePreferences",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "PreferenceType",
                table: "RecipePreferences",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MeatPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PreferenceType = table.Column<int>(type: "integer", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeatPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeafoodPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PreferenceType = table.Column<int>(type: "integer", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeafoodPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VegetarianPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PreferenceType = table.Column<int>(type: "integer", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VegetarianPreferences", x => x.Id);
                });
        }
    }
}

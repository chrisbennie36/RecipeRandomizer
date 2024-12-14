using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeRandomizer.Api.WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class RecipePrefereceTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecipeType",
                table: "RecipePreferences",
                newName: "Type");

            migrationBuilder.AddColumn<bool>(
                name: "Excluded",
                table: "RecipePreferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Translations",
                table: "RecipePreferences",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Excluded",
                table: "RecipePreferences");

            migrationBuilder.DropColumn(
                name: "Translations",
                table: "RecipePreferences");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "RecipePreferences",
                newName: "RecipeType");
        }
    }
}

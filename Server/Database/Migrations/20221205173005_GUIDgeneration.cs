using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemeIT.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class GUIDgeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Memes",
                newName: "MemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MemeId",
                table: "Memes",
                newName: "Id");
        }
    }
}

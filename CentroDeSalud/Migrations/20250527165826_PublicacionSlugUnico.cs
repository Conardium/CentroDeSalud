using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentroDeSalud.Migrations
{
    /// <inheritdoc />
    public partial class PublicacionSlugUnico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_Slug",
                table: "Publicaciones",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Publicaciones_Slug",
                table: "Publicaciones");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentroDeSalud.Migrations
{
    /// <inheritdoc />
    public partial class ModLoginExterno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsuariosLoginExterno_UsuarioId",
                table: "UsuariosLoginExterno");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosLoginExterno_UsuarioId",
                table: "UsuariosLoginExterno",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsuariosLoginExterno_UsuarioId",
                table: "UsuariosLoginExterno");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosLoginExterno_UsuarioId",
                table: "UsuariosLoginExterno",
                column: "UsuarioId",
                unique: true);
        }
    }
}

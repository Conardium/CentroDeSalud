using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentroDeSalud.Migrations
{
    /// <inheritdoc />
    public partial class Publicaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Publicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Cuerpo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resumen = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Destacada = table.Column<bool>(type: "bit", nullable: false),
                    EstadoPublicacion = table.Column<int>(type: "int", nullable: false),
                    ImagenURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_UsuarioId",
                table: "Publicaciones",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Publicaciones");
        }
    }
}

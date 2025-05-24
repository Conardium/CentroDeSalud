using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentroDeSalud.Migrations
{
    /// <inheritdoc />
    public partial class PreguntaRespuestaForo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreguntasForos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "DATE", nullable: false),
                    EstadoPregunta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreguntasForos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreguntasForos_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RespuestasForos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreguntaForoId = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasForos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespuestasForos_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RespuestasForos_PreguntasForos_PreguntaForoId",
                        column: x => x.PreguntaForoId,
                        principalTable: "PreguntasForos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreguntasForos_PacienteId",
                table: "PreguntasForos",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasForos_MedicoId",
                table: "RespuestasForos",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasForos_PreguntaForoId",
                table: "RespuestasForos",
                column: "PreguntaForoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespuestasForos");

            migrationBuilder.DropTable(
                name: "PreguntasForos");
        }
    }
}

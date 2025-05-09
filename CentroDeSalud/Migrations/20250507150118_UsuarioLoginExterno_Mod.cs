using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentroDeSalud.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioLoginExterno_Mod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "UsuariosLoginExterno",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "UsuariosLoginExterno",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiraEn",
                table: "UsuariosLoginExterno",
                type: "datetime2(0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "UsuariosLoginExterno");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "UsuariosLoginExterno");

            migrationBuilder.DropColumn(
                name: "TokenExpiraEn",
                table: "UsuariosLoginExterno");
        }
    }
}

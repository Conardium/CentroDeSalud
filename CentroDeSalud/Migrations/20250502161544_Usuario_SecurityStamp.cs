using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentroDeSalud.Migrations
{
    /// <inheritdoc />
    public partial class Usuario_SecurityStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Usuarios");
        }
    }
}

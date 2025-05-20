using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentroDeSalud.Migrations
{
    /// <inheritdoc />
    public partial class CitasMod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Sincronizada",
                table: "Citas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sincronizada",
                table: "Citas");
        }
    }
}

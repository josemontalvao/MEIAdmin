using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEIAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddCGCToCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CGC",
                table: "Clientes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InscEstadual",
                table: "Clientes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CGC",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "InscEstadual",
                table: "Clientes");
        }
    }
}

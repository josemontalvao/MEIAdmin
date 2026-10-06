using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEIAdmin.Migrations
{
    /// <inheritdoc />
    public partial class ImagensNoBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssinaturaClienteBase64",
                table: "OrdensServico",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssinaturaClienteBase64",
                table: "OrdensServico");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEIAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarAcessoColaborador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Colaboradores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Perfil",
                table: "Colaboradores",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "Colaboradores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Login",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "Perfil",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "Senha",
                table: "Colaboradores");
        }
    }
}

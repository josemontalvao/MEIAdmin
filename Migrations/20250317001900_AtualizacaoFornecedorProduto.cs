using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MEIAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoFornecedorProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FornecedoresProdutos_Fornecedores_FornecedorId",
                table: "FornecedoresProdutos");

            migrationBuilder.DropForeignKey(
                name: "FK_FornecedoresProdutos_Produtos_ProdutoId",
                table: "FornecedoresProdutos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_FornecedorId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "FornecedorId",
                table: "Produtos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInstalacao",
                table: "Dispositivos",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddForeignKey(
                name: "FK_FornecedoresProdutos_Fornecedores_FornecedorId",
                table: "FornecedoresProdutos",
                column: "FornecedorId",
                principalTable: "Fornecedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FornecedoresProdutos_Produtos_ProdutoId",
                table: "FornecedoresProdutos",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FornecedoresProdutos_Fornecedores_FornecedorId",
                table: "FornecedoresProdutos");

            migrationBuilder.DropForeignKey(
                name: "FK_FornecedoresProdutos_Produtos_ProdutoId",
                table: "FornecedoresProdutos");

            migrationBuilder.AddColumn<int>(
                name: "FornecedorId",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInstalacao",
                table: "Dispositivos",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_FornecedorId",
                table: "Produtos",
                column: "FornecedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_FornecedoresProdutos_Fornecedores_FornecedorId",
                table: "FornecedoresProdutos",
                column: "FornecedorId",
                principalTable: "Fornecedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FornecedoresProdutos_Produtos_ProdutoId",
                table: "FornecedoresProdutos",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos",
                column: "FornecedorId",
                principalTable: "Fornecedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

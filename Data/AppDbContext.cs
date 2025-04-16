using Microsoft.EntityFrameworkCore;
using MEIAdmin.Models;

namespace MEIAdmin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tabelas do banco de dados
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Dispositivo> Dispositivos { get; set; }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<FornecedorProduto> FornecedoresProdutos { get; set; }
        public DbSet<ContaPagar> ContasPagar { get; set; }
        public DbSet<ContaReceber> ContasReceber { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do relacionamento muitos-para-muitos entre Fornecedor e Produto
            modelBuilder.Entity<FornecedorProduto>()
                .HasKey(fp => new { fp.FornecedorId, fp.ProdutoId });

            modelBuilder.Entity<FornecedorProduto>()
                .HasOne(fp => fp.Fornecedor)
                .WithMany(f => f.FornecedorProdutos)
                .HasForeignKey(fp => fp.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FornecedorProduto>()
                .HasOne(fp => fp.Produto)
                .WithMany(p => p.FornecedorProdutos)
                .HasForeignKey(fp => fp.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
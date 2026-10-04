using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MEIAdmin.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100)]
        public string? Nome { get; set; }  

        [Required]
        public int Quantidade { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoCompra { get; set; } 

        // AJUSTE 1: Aumentei o limite para 1000% para você poder lucrar em peças baratas
        [Required]
        [Range(0, 1000, ErrorMessage = "A margem de lucro deve estar entre 0% e 1000%.")]
        public double MargemLucro { get; set; } = 30; 

        [NotMapped] 
        public decimal PrecoVenda => PrecoCompra * (1 + (decimal)MargemLucro / 100);

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataCompra { get; set; } 

        [Required]
        [StringLength(20)]
        public string? UnidadeMedida { get; set; } 

        [StringLength(50)]
        public string? Categoria { get; set; } 

        public bool UsadoInternamente { get; set; } 

        // AJUSTE 2: Campo para ocultar produtos que saíram de linha
        public bool Ativo { get; set; } = true;

        // Relacionamentos
        public List<FornecedorProduto> FornecedorProdutos { get; set; } = new List<FornecedorProduto>();
    }
}

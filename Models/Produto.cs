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

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }  // Ex: Câmera IP, Sensor de presença

        [Required]
        public int Quantidade { get; set; } // Ex: 10 unidades

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoCompra { get; set; } // Ex: 10.00

        [Required]
        [Range(0, 100, ErrorMessage = "A margem de lucro deve estar entre 0% e 100%.")]
        public double MargemLucro { get; set; } = 30; // Margem padrão de 30%

        [NotMapped] // Calculado em tempo de execução
        public decimal PrecoVenda => PrecoCompra * (1 + (decimal)MargemLucro / 100);

        [Required]
        public DateTime DataCompra { get; set; } // Será preenchido manualmente

        [Required]
        [StringLength(20)]
        public string UnidadeMedida { get; set; } // Ex: Unidade, Caixa, Metro

        [StringLength(50)]
        public string Categoria { get; set; } // Ex: CFTV, Automação, Escritório

        public bool UsadoInternamente { get; set; } // True = Uso interno, False = Venda

        // Relacionamento muitos-para-muitos com Fornecedor
        public List<FornecedorProduto> FornecedorProdutos { get; set; } = new List<FornecedorProduto>();
    }
}

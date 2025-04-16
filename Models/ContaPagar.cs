using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MEIAdmin.Models
{
    public class ContaPagar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Valor { get; set; }

        public bool Pago { get; set; }

        [Required]
        public DateTime DataVencimento { get; set; }

        public DateTime? DataPagamento { get; set; }

        [Required]
        public string Status { get; set; } = "Pendente"; // Pendente, Pago, Atrasado

        public string? Descricao { get; set; }

        // Relacionamento com Fornecedor (Opcional)
        public int? FornecedorId { get; set; }
        [ForeignKey("FornecedorId")]
        public Fornecedor? Fornecedor { get; set; }

        // Relacionamento com Colaborador (Opcional)
        public int? ColaboradorId { get; set; }
        [ForeignKey("ColaboradorId")]
        public Colaborador? Colaborador { get; set; }
    }    
}

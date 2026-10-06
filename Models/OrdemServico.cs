using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MEIAdmin.Models
{
    public class OrdemServico
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A data de abertura é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataAbertura { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Selecione o cliente.")]
        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        [ForeignKey("Colaborador")]
        public int? ColaboradorId { get; set; }
        public Colaborador? Colaborador { get; set; }

        [StringLength(50)]
        public string TipoManutencao { get; set; } = "Preventiva";

        [Required(ErrorMessage = "Descreva a instrução do serviço.")]
        [StringLength(500)]
        public string DescricaoProblema { get; set; } = string.Empty;

        public string Status { get; set; } = "Pendente";

        [DataType(DataType.DateTime)]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DataFim { get; set; }

        public string? ServicoExecutado { get; set; }
        public string? PecasUtilizadas { get; set; }
        public string? ObservacoesTecnicas { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TempoGastoHoras { get; set; }

        // CAMPO BLINDADO: A assinatura fica gravada direto no banco de dados da Hostinger!
        [Column(TypeName = "longtext")]
        public string? AssinaturaClienteBase64 { get; set; }

        public List<FotoOrdemServico> Fotos { get; set; } = new List<FotoOrdemServico>();
    }

    public class FotoOrdemServico
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OrdemServico")]
        public int OrdemServicoId { get; set; }
        public OrdemServico? OrdemServico { get; set; }

        // CAMPO BLINDADO: A foto fica gravada direto no banco de dados da Hostinger!
        [Required]
        [Column(TypeName = "longtext")]
        public string CaminhoArquivo { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Legenda { get; set; }
    }
}

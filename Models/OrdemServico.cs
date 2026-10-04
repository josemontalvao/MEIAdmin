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

        // --- 1. DADOS DE DESPACHO (PREENCHIDOS POR VOCÊ NA CENTRAL) ---
        [Required(ErrorMessage = "A data de abertura é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataAbertura { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Selecione o cliente.")]
        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        [ForeignKey("Colaborador")]
        public int? ColaboradorId { get; set; }
        public Colaborador? Colaborador { get; set; } // O Técnico Responsável

        [StringLength(50)]
        public string TipoManutencao { get; set; } = "Preventiva"; // Preventiva, Corretiva, Instalação

        [Required(ErrorMessage = "Descreva a instrução do serviço.")]
        [StringLength(500)]
        public string DescricaoProblema { get; set; } = string.Empty; // O que o técnico deve fazer

        public string Status { get; set; } = "Pendente"; // Pendente, Em Andamento, Concluída, Cancelada

        // --- 2. EXECUÇÃO EM CAMPO (PREENCHIDO PELO TÉCNICO NO CELULAR) ---
        [DataType(DataType.DateTime)]
        public DateTime? DataInicio { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DataFim { get; set; }

        public string? ServicoExecutado { get; set; } // O que o técnico realmente fez
        public string? PecasUtilizadas { get; set; }  // Cabos, conectores, fontes, etc.
        public string? ObservacoesTecnicas { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TempoGastoHoras { get; set; }

        // --- 3. EVIDÊNCIAS FOTOGRÁFICAS COM LEGENDAS ---
        public List<FotoOrdemServico> Fotos { get; set; } = new List<FotoOrdemServico>();
    }

    // Tabela filha para guardar as fotos tiradas pelo celular e a legenda de cada uma
    public class FotoOrdemServico
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OrdemServico")]
        public int OrdemServicoId { get; set; }
        public OrdemServico? OrdemServico { get; set; }

        [Required]
        public string CaminhoArquivo { get; set; } = string.Empty; // Caminho da foto no servidor

        [StringLength(200)]
        public string? Legenda { get; set; } // A legenda da foto digitada pelo técnico
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEIAdmin.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string RazaoSocial { get; set; } = string.Empty;

        public string Contato { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        public string CGC { get; set; } = string.Empty;
        public string InscEstadual { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de cadastro é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataCadastro { get; set; }

        // Endereço do cliente
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;

        // Novo campo para ativar/desativar cliente
        public bool Ativo { get; set; } = true;

        // Relacionamento: Um cliente pode ter vários dispositivos
        public List<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();
    }
}

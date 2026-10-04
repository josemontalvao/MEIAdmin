using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEIAdmin.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A Razão Social ou Nome é obrigatório.")]
        public string RazaoSocial { get; set; } = string.Empty;

        public string? Contato { get; set; }
        public string? Telefone { get; set; }

        // E-mail opcional (com a interrogação para não travar)
        public string? Email { get; set; }

        // Documentos opcionais
        public string? CpfCnpj { get; set; }
        public string? InscEstadual { get; set; }

        [Required(ErrorMessage = "A data de cadastro é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataCadastro { get; set; }

        // Endereço opcional
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? CEP { get; set; }

        // Campo para ativar/desativar cliente
        public bool Ativo { get; set; } = true;

        // Relacionamentos
        public List<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();
    }
}


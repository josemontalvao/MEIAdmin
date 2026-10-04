using System;

namespace MEIAdmin.Models
{
    public class Colaborador
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Cargo { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }

        // --- CREDENCIAIS DE ACESSO AO SISTEMA / APP DO CELULAR ---
        public string? Login { get; set; }
        public string? Senha { get; set; }
        public string Perfil { get; set; } = "Tecnico"; // "Admin" para você, "Tecnico" para a equipe de campo

        // Endereço do Colaborador
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? CEP { get; set; }

        // Datas de contratação e demissão
        public DateTime DataContratacao { get; set; }
        public DateTime? DataDemissao { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MEIAdmin.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A Razão Social é obrigatória.")]
        public string? RazaoSocial { get; set; }  

        // NOVO: CNPJ é vital para cadastro de fornecedores
        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        public string? CNPJ { get; set; }

        public string? Contato { get; set; }  
        public string? Telefone { get; set; }
        public string? Email { get; set; }

        // Endereço do Fornecedor  
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? CEP { get; set; }

        // Inicializando como true (Ativo por padrão)
        public bool Ativo { get; set; } = true;

        // Relacionamento muitos-para-muitos com Produto  
        public List<FornecedorProduto> FornecedorProdutos { get; set; } = new List<FornecedorProduto>();
    }
}
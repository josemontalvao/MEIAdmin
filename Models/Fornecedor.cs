namespace MEIAdmin.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }  // Substitui Nome  
        public string Contato { get; set; }  // Pessoa responsável pelo atendimento  
        public string Telefone { get; set; }
        public string Email { get; set; }

        // Endereço do Fornecedor  
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }

        public bool Ativo { get; set; }

        // Relacionamento muitos-para-muitos com Produto  
        public List<FornecedorProduto> FornecedorProdutos { get; set; } = new List<FornecedorProduto>();
    }
}
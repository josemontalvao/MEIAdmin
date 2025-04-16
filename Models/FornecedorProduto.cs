using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MEIAdmin.Models
{
    public class FornecedorProduto
    {
        [Key, Column(Order = 1)]
        public int FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; }

        [Key, Column(Order = 2)]
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
    }
}
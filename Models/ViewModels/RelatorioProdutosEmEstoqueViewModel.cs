using System;

namespace MEIAdmin.Models.ViewModels
{
    public class RelatorioProdutosEmEstoqueViewModel
    {
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public string UnidadeMedida { get; set; }
        public DateTime DataCompra { get; set; }
    }
}

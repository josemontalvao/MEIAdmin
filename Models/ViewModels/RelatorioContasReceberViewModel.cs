using System;
using System.Collections.Generic;
using MEIAdmin.Models;

namespace MEIAdmin.Models.ViewModels
{
    public class RelatorioContasReceberViewModel
    {
        public DateTime DataVencimento { get; set; }
        public decimal Total { get; set; }
        public List<ContaReceber> Contas { get; set; }
    }
}

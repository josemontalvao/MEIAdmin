using System;
using System.Collections.Generic;
using MEIAdmin.Models;

namespace MEIAdmin.Models.ViewModels
{
    public class BalancoMensalViewModel
    {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public decimal TotalRecebido { get; set; }
        public decimal TotalPago { get; set; }
        public decimal SaldoLiquido => TotalRecebido - TotalPago;

        public List<ContaReceber> Recebimentos { get; set; } = new List<ContaReceber>();
        public List<ContaPagar> Pagamentos { get; set; } = new List<ContaPagar>();
    }
}
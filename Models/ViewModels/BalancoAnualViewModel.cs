using System;
using System.Collections.Generic;

namespace MEIAdmin.Models.ViewModels
{
    public class BalancoAnualViewModel
    {
        public int Ano { get; set; }
        public decimal TotalRecebidoAno { get; set; }
        public decimal TotalPagoAno { get; set; }
        public decimal SaldoLiquidoAno => TotalRecebidoAno - TotalPagoAno;

        // Lista dos 12 meses consolidados (Janeiro a Dezembro)
        public List<MesResumoItem> Meses { get; set; } = new List<MesResumoItem>();
    }

    public class MesResumoItem
    {
        public int MesNumero { get; set; }
        public string MesNome { get; set; } = string.Empty;
        public decimal Entradas { get; set; }
        public decimal Saidas { get; set; }
        public decimal Saldo => Entradas - Saidas;
    }
}
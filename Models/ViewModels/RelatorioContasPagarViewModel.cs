namespace MEIAdmin.Models.ViewModels
{
    public class RelatorioContasPagarViewModel
    {
        public DateTime DataVencimento { get; set; }
        public decimal Total { get; set; }
        public List<ContaPagar> Contas { get; set; } = new List<ContaPagar>();
    }
}

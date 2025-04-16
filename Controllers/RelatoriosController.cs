using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MEIAdmin.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _context;

        public RelatoriosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ContasPagarPendentes()
        {
            var contasPagar = await _context.ContasPagar
                .Where(c => c.Status != "Pago")
                .GroupBy(c => c.DataVencimento)
                .Select(g => new RelatorioContasPagarViewModel
                {
                    DataVencimento = g.Key,
                    Total = g.Sum(c => c.Valor),
                    Contas = g.ToList()
                })
                .OrderBy(g => g.DataVencimento)
                .ToListAsync();

            return View("RelatorioContasPagar", contasPagar);
        }

        public async Task<IActionResult> ContasReceberPendentes()
        {
            var contasReceber = await _context.ContasReceber
                .Where(c => c.Status != "Pago")
                .GroupBy(c => c.DataVencimento)
                .Select(g => new RelatorioContasReceberViewModel
                {
                    DataVencimento = g.Key,
                    Total = g.Sum(c => c.Valor),
                    Contas = g.ToList()
                })
                .OrderBy(g => g.DataVencimento)
                .ToListAsync();

            return View("RelatorioContasReceber", contasReceber);
        }

        public async Task<IActionResult> ProdutosEmEstoque()
        {
            var produtos = await _context.Produtos
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(p => new RelatorioProdutosEmEstoqueViewModel
                {
                    NomeProduto = p.Nome,
                    Quantidade = p.Quantidade,
                    UnidadeMedida = p.UnidadeMedida,
                    DataCompra = p.DataCompra
                })
                .ToListAsync();

            return View("RelatorioProdutos", produtos);
        }
    }
}
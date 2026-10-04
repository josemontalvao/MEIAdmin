using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models.ViewModels;
using System;
using System.Collections.Generic;
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

        // GET: Relatorios/ContasPagarPendentes
        public async Task<IActionResult> ContasPagarPendentes(int? mes, int? ano)
        {
            var query = _context.ContasPagar
                .Include(c => c.Fornecedor)
                .Include(c => c.Colaborador)
                .Where(c => c.Status != "Pago");

            if (mes.HasValue && mes.Value > 0)
            {
                query = query.Where(c => c.DataVencimento.Month == mes.Value);
            }

            if (ano.HasValue && ano.Value > 0)
            {
                query = query.Where(c => c.DataVencimento.Year == ano.Value);
            }

            var contasPagar = await query
                .GroupBy(c => c.DataVencimento)
                .Select(g => new RelatorioContasPagarViewModel
                {
                    DataVencimento = g.Key,
                    Total = g.Sum(c => c.Valor),
                    Contas = g.ToList()
                })
                .OrderBy(g => g.DataVencimento)
                .ToListAsync();

            ViewBag.MesSelecionado = mes;
            ViewBag.AnoSelecionado = ano ?? DateTime.Now.Year;

            return View("RelatorioContasPagar", contasPagar);
        }

        // GET: Relatorios/ContasReceberPendentes
        public async Task<IActionResult> ContasReceberPendentes(int? mes, int? ano)
        {
            var query = _context.ContasReceber
                .Include(c => c.Cliente) 
                .Where(c => c.Status != "Pago" && c.Status != "Recebido");

            if (mes.HasValue && mes.Value > 0)
            {
                query = query.Where(c => c.DataVencimento.Month == mes.Value);
            }

            if (ano.HasValue && ano.Value > 0)
            {
                query = query.Where(c => c.DataVencimento.Year == ano.Value);
            }

            var contasReceber = await query
                .GroupBy(c => c.DataVencimento)
                .Select(g => new RelatorioContasReceberViewModel
                {
                    DataVencimento = g.Key,
                    Total = g.Sum(c => c.Valor),
                    Contas = g.ToList()
                })
                .OrderBy(g => g.DataVencimento)
                .ToListAsync();

            ViewBag.MesSelecionado = mes;
            ViewBag.AnoSelecionado = ano ?? DateTime.Now.Year;

            return View("RelatorioContasReceber", contasReceber);
        }

        // GET: Relatorios/ProdutosEmEstoque
        public async Task<IActionResult> ProdutosEmEstoque()
        {
            var produtos = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.Quantidade > 0)
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

        // GET: Relatorios/ContasPagas
        public async Task<IActionResult> ContasPagas(int? mes, int? ano)
        {
            var query = _context.ContasPagar
                .Include(c => c.Fornecedor)
                .Include(c => c.Colaborador)
                .Where(c => c.Status == "Pago");

            if (mes.HasValue && mes.Value > 0)
            {
                query = query.Where(c => c.DataPagamento.HasValue && c.DataPagamento.Value.Month == mes.Value);
            }

            if (ano.HasValue && ano.Value > 0)
            {
                query = query.Where(c => c.DataPagamento.HasValue && c.DataPagamento.Value.Year == ano.Value);
            }

            var contasPagas = await query
                .OrderBy(c => c.DataPagamento)
                .ToListAsync();

            ViewBag.MesSelecionado = mes;
            ViewBag.AnoSelecionado = ano ?? DateTime.Now.Year;

            return View("RelatorioContasPagas", contasPagas);
        }

        // GET: Relatorios/ContasRecebidas
        public async Task<IActionResult> ContasRecebidas(int? mes, int? ano)
        {
            var query = _context.ContasReceber
                .Include(c => c.Cliente)
                .Where(c => c.Status == "Recebido" || c.Status == "Pago");

            if (mes.HasValue && mes.Value > 0)
            {
                query = query.Where(c => c.DataRecebimento.HasValue && c.DataRecebimento.Value.Month == mes.Value);
            }

            if (ano.HasValue && ano.Value > 0)
            {
                query = query.Where(c => c.DataRecebimento.HasValue && c.DataRecebimento.Value.Year == ano.Value);
            }

            var contasRecebidas = await query
                .OrderBy(c => c.DataRecebimento)
                .ToListAsync();

            ViewBag.MesSelecionado = mes;
            ViewBag.AnoSelecionado = ano ?? DateTime.Now.Year;

            return View("RelatorioContasRecebidas", contasRecebidas);
        }

        // GET: Relatorios/BalancoMensal
        public async Task<IActionResult> BalancoMensal(int? mes, int? ano)
        {
            int mesFiltro = mes ?? DateTime.Now.Month;
            int anoFiltro = ano ?? DateTime.Now.Year;

            var recebimentos = await _context.ContasReceber
                .Include(c => c.Cliente)
                .Where(c => (c.Status == "Recebido" || c.Status == "Pago") &&
                            c.DataRecebimento.HasValue &&
                            c.DataRecebimento.Value.Month == mesFiltro &&
                            c.DataRecebimento.Value.Year == anoFiltro)
                .OrderBy(c => c.DataRecebimento)
                .ToListAsync();

            var pagamentos = await _context.ContasPagar
                .Include(c => c.Fornecedor)
                .Where(c => c.Status == "Pago" &&
                            c.DataPagamento.HasValue &&
                            c.DataPagamento.Value.Month == mesFiltro &&
                            c.DataPagamento.Value.Year == anoFiltro)
                .OrderBy(c => c.DataPagamento)
                .ToListAsync();

            var viewModel = new BalancoMensalViewModel
            {
                Mes = mesFiltro,
                Ano = anoFiltro,
                Recebimentos = recebimentos,
                Pagamentos = pagamentos,
                TotalRecebido = recebimentos.Sum(r => r.Valor),
                TotalPago = pagamentos.Sum(p => p.Valor)
            };

            return View("RelatorioBalancoMensal", viewModel);
        }

        // ==========================================
        // NOVO: BALANÇO FINANCEIRO ANUAL (JANEIRO A DEZEMBRO)
        // ==========================================
        // GET: Relatorios/BalancoAnual
        public async Task<IActionResult> BalancoAnual(int? ano)
        {
            int anoFiltro = ano ?? DateTime.Now.Year;

            var nomesMeses = new Dictionary<int, string>
            {
                { 1, "Janeiro" }, { 2, "Fevereiro" }, { 3, "Março" }, { 4, "Abril" },
                { 5, "Maio" }, { 6, "Junho" }, { 7, "Julho" }, { 8, "Agosto" },
                { 9, "Setembro" }, { 10, "Outubro" }, { 11, "Novembro" }, { 12, "Dezembro" }
            };

            // Busca todas as entradas e saídas do ano selecionado
            var recebimentosAno = await _context.ContasReceber
                .Where(c => (c.Status == "Recebido" || c.Status == "Pago") &&
                            c.DataRecebimento.HasValue &&
                            c.DataRecebimento.Value.Year == anoFiltro)
                .ToListAsync();

            var pagamentosAno = await _context.ContasPagar
                .Where(c => c.Status == "Pago" &&
                            c.DataPagamento.HasValue &&
                            c.DataPagamento.Value.Year == anoFiltro)
                .ToListAsync();

            var listaMeses = new List<MesResumoItem>();

            // Consolida os 12 meses
            for (int m = 1; m <= 12; m++)
            {
                decimal entradas = recebimentosAno.Where(r => r.DataRecebimento.Value.Month == m).Sum(r => r.Valor);
                decimal saidas = pagamentosAno.Where(p => p.DataPagamento.Value.Month == m).Sum(p => p.Valor);

                listaMeses.Add(new MesResumoItem
                {
                    MesNumero = m,
                    MesNome = nomesMeses[m],
                    Entradas = entradas,
                    Saidas = saidas
                });
            }

            var viewModel = new BalancoAnualViewModel
            {
                Ano = anoFiltro,
                TotalRecebidoAno = recebimentosAno.Sum(r => r.Valor),
                TotalPagoAno = pagamentosAno.Sum(p => p.Valor),
                Meses = listaMeses
            };

            return View("RelatorioBalancoAnual", viewModel);
        }
    }
}
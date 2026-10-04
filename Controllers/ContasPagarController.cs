using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MEIAdmin.Controllers
{
    public class ContasPagarController : Controller
    {
        private readonly AppDbContext _context;

        public ContasPagarController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ContasPagar (Ordenado cronologicamente por vencimento)
        public async Task<IActionResult> Index()
        {
            var contasPagar = await _context.ContasPagar
                .Include(c => c.Fornecedor)
                .Include(c => c.Colaborador)
                .OrderBy(c => c.DataVencimento)
                .ToListAsync();
            return View("Index", contasPagar);
        }

        // GET: ContasPagar/Create
        public IActionResult Create()
        {
            CarregarViewBag(); 
            return View();
        }

        // POST: ContasPagar/Create (COM GERADOR AUTOMÁTICO DE PARCELAS)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,Valor,DataVencimento,DataPagamento,Status,FornecedorId,ColaboradorId")] ContaPagar contaPagar, int parcelas = 1)
        {
            if (ModelState.IsValid)
            {
                // Se for boleto único (à vista)
                if (parcelas <= 1)
                {
                    _context.Add(contaPagar);
                }
                else
                {
                    // Se for faturado/parcelado, gera todos os boletos automaticamente
                    decimal valorTotal = contaPagar.Valor;
                    decimal valorParcelaBase = Math.Round(valorTotal / parcelas, 2);
                    string statusOriginal = contaPagar.Status ?? "Não Pago";
                    DateTime? dataPagtoOriginal = contaPagar.DataPagamento;

                    for (int i = 1; i <= parcelas; i++)
                    {
                        // Ajusta centavos na última parcela
                        decimal valorAtual = (i == parcelas) 
                            ? (valorTotal - (valorParcelaBase * (parcelas - 1))) 
                            : valorParcelaBase;

                        // Se marcou como "Pago", apenas a 1ª parcela fica quitada
                        // As parcelas futuras já nascem como "Não Pago" (Pendente)
                        string statusParcela = (i == 1 && statusOriginal == "Pago") ? "Pago" : "Não Pago";
                        DateTime? dataPagtoParcela = (i == 1 && statusOriginal == "Pago") ? dataPagtoOriginal : null;

                        var novaParcela = new ContaPagar
                        {
                            FornecedorId = contaPagar.FornecedorId,
                            ColaboradorId = contaPagar.ColaboradorId,
                            Descricao = $"{contaPagar.Descricao} (Parcela {i}/{parcelas})",
                            Valor = valorAtual,
                            DataVencimento = contaPagar.DataVencimento.AddMonths(i - 1),
                            DataPagamento = dataPagtoParcela,
                            Status = statusParcela
                        };

                        _context.Add(novaParcela);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CarregarViewBag();
            return View(contaPagar);
        }

        // GET: ContasPagar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contaPagar = await _context.ContasPagar.FindAsync(id);
            if (contaPagar == null) return NotFound();

            CarregarViewBag();
            return View(contaPagar);
        }

        // POST: ContasPagar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,Valor,DataVencimento,DataPagamento,Status,FornecedorId,ColaboradorId")] ContaPagar contaPagar)
        {
            if (id != contaPagar.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contaPagar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContaPagarExists(contaPagar.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            CarregarViewBag();
            return View(contaPagar);
        }

        // GET: ContasPagar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contaPagar = await _context.ContasPagar
                .Include(c => c.Fornecedor)
                .Include(c => c.Colaborador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contaPagar == null) return NotFound();

            return View(contaPagar);
        }

        // POST: ContasPagar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contaPagar = await _context.ContasPagar.FindAsync(id);
            if (contaPagar != null)
            {
                _context.ContasPagar.Remove(contaPagar);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ContaPagarExists(int id)
        {
            return _context.ContasPagar.Any(e => e.Id == id);
        }

        // Método para carregar fornecedores e colaboradores no ViewBag
        private void CarregarViewBag()
        {
            ViewBag.Fornecedores = _context.Fornecedores?.ToList() ?? new List<Fornecedor>();
            ViewBag.Colaboradores = _context.Colaboradores?.ToList() ?? new List<Colaborador>();
        }
    }
}
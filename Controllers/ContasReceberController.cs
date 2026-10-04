using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MEIAdmin.Controllers
{
    public class ContasReceberController : Controller
    {
        private readonly AppDbContext _context;

        public ContasReceberController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ContasReceber (Ordenado por data de vencimento)
        public async Task<IActionResult> Index()
        {
            var contasReceber = await _context.ContasReceber
                .Include(c => c.Cliente)
                .OrderBy(c => c.DataVencimento) // <-- Ordena cronologicamente
                .ToListAsync();
            return View("Index", contasReceber);
        }

        // GET: ContasReceber/Create
        public IActionResult Create()
        {
            CarregarViewBag();
            return View();
        }

        // POST: ContasReceber/Create (COM GERADOR AUTOMÁTICO DE PARCELAS)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,Valor,DataVencimento,DataRecebimento,Status,ClienteId")] ContaReceber contaReceber, int parcelas = 1)
        {
            if (!ModelState.IsValid)
            {
                CarregarViewBag();
                return View(contaReceber);
            }

            // Se for parcela única (à vista)
            if (parcelas <= 1)
            {
                _context.Add(contaReceber);
            }
            else
            {
                // Se for parcelado, gera todas as parcelas automaticamente
                decimal valorTotal = contaReceber.Valor;
                decimal valorParcelaBase = Math.Round(valorTotal / parcelas, 2);
                string statusOriginal = contaReceber.Status ?? "Pendente";
                DateTime? dataRecebtoOriginal = contaReceber.DataRecebimento;

                for (int i = 1; i <= parcelas; i++)
                {
                    // Ajusta a diferença de centavos na última parcela
                    decimal valorAtual = (i == parcelas) 
                        ? (valorTotal - (valorParcelaBase * (parcelas - 1))) 
                        : valorParcelaBase;

                    // Se marcou como "Recebido", apenas a 1ª parcela fica recebida (sinal/entrada)
                    // As parcelas futuras já nascem como "Pendente"
                    string statusParcela = (i == 1 && statusOriginal == "Recebido") ? "Recebido" : "Pendente";
                    DateTime? dataRecebtoParcela = (i == 1 && statusOriginal == "Recebido") ? dataRecebtoOriginal : null;

                    var novaParcela = new ContaReceber
                    {
                        ClienteId = contaReceber.ClienteId,
                        Descricao = $"{contaReceber.Descricao} (Parcela {i}/{parcelas})",
                        Valor = valorAtual,
                        DataVencimento = contaReceber.DataVencimento.AddMonths(i - 1),
                        DataRecebimento = dataRecebtoParcela,
                        Status = statusParcela
                    };

                    _context.Add(novaParcela);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ContasReceber/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contaReceber = await _context.ContasReceber.FindAsync(id);
            if (contaReceber == null) return NotFound();

            CarregarViewBag();
            return View(contaReceber);
        }

        // POST: ContasReceber/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descricao,Valor,DataVencimento,DataRecebimento,Status,ClienteId")] ContaReceber contaReceber)
        {
            if (id != contaReceber.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                CarregarViewBag();
                return View(contaReceber);
            }

            _context.Update(contaReceber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ContasReceber/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contaReceber = await _context.ContasReceber
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contaReceber == null) return NotFound();

            return View(contaReceber);
        }

        // POST: ContasReceber/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contaReceber = await _context.ContasReceber.FindAsync(id);
            if (contaReceber != null)
            {
                _context.ContasReceber.Remove(contaReceber);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ContaReceberExists(int id)
        {
            return _context.ContasReceber.Any(e => e.Id == id);
        }

        // Método para carregar clientes no ViewBag corretamente
        private void CarregarViewBag()
        {
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "RazaoSocial");
        }
    }
}


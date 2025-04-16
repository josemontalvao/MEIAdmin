using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models;
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

        // GET: ContasPagar
        public async Task<IActionResult> Index()
        {
            var contasPagar = await _context.ContasPagar
                .Include(c => c.Fornecedor)
                .Include(c => c.Colaborador)
                .ToListAsync();
            return View("Index", contasPagar);
        }

        // GET: ContasPagar/Create
        public IActionResult Create()
        {
            CarregarViewBag();
            return View();
        }

        // POST: ContasPagar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,Valor,DataVencimento,DataPagamento,Status,FornecedorId,ColaboradorId")] ContaPagar contaPagar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contaPagar);
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
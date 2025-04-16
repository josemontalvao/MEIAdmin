using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models;
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

        // GET: ContasReceber
        public async Task<IActionResult> Index()
        {
            var contasReceber = await _context.ContasReceber
                .Include(c => c.Cliente)
                .ToListAsync();
            return View("Index", contasReceber);
        }

        // GET: ContasReceber/Create
        public IActionResult Create()
        {
            CarregarViewBag();
            return View();
        }

        // POST: ContasReceber/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descricao,Valor,DataVencimento,DataRecebimento,Status,ClienteId")] ContaReceber contaReceber)
        {
            if (!ModelState.IsValid)
            {
                CarregarViewBag();
                return View(contaReceber);
            }

            _context.Add(contaReceber);
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

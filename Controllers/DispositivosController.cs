using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models;

namespace MEIAdmin.Controllers
{
    public class DispositivosController : Controller
    {
        private readonly AppDbContext _context;

        public DispositivosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dispositivos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Dispositivos.Include(d => d.Cliente);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dispositivos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dispositivo = await _context.Dispositivos
                .Include(d => d.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dispositivo == null) return NotFound();

            return View(dispositivo);
        }

        // GET: Dispositivos/Create
        // Agora aceita clienteId para já vir preenchido com o cliente certo!
        public IActionResult Create(int? clienteId)
        {
            CarregarListas(clienteId);
            
            var dispositivo = new Dispositivo
            {
                ClienteId = clienteId ?? 0,
                DataInstalacao = DateTime.Today
            };

            return View(dispositivo);
        }

        // POST: Dispositivos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Modelo,NumeroSerie,DataInstalacao,ClienteId")] Dispositivo dispositivo, int? produtoId)
        {
            if (ModelState.IsValid)
            {
                // Se escolheu um produto do estoque, abate 1 unidade!
                if (produtoId.HasValue && produtoId.Value > 0)
                {
                    var produto = await _context.Produtos.FindAsync(produtoId.Value);
                    if (produto != null && produto.Quantidade > 0)
                    {
                        produto.Quantidade -= 1;
                        _context.Update(produto);

                        // Se não digitou nome ou modelo, preenche com o nome do produto
                        if (string.IsNullOrWhiteSpace(dispositivo.Nome))
                            dispositivo.Nome = produto.Nome;

                        if (string.IsNullOrWhiteSpace(dispositivo.Modelo))
                            dispositivo.Modelo = produto.Nome;
                    }
                }

                _context.Add(dispositivo);
                await _context.SaveChangesAsync();

                // Volta direto para a lista de dispositivos do próprio cliente!
                return RedirectToAction("Dispositivos", "Cliente", new { id = dispositivo.ClienteId });
            }

            CarregarListas(dispositivo.ClienteId, produtoId);
            return View(dispositivo);
        }

        // GET: Dispositivos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dispositivo = await _context.Dispositivos.FindAsync(id);
            if (dispositivo == null) return NotFound();

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "RazaoSocial", dispositivo.ClienteId);
            return View(dispositivo);
        }

        // POST: Dispositivos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Modelo,NumeroSerie,DataInstalacao,ClienteId")] Dispositivo dispositivo)
        {
            if (id != dispositivo.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dispositivo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DispositivoExists(dispositivo.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Dispositivos", "Cliente", new { id = dispositivo.ClienteId });
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "RazaoSocial", dispositivo.ClienteId);
            return View(dispositivo);
        }

        // GET: Dispositivos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dispositivo = await _context.Dispositivos
                .Include(d => d.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dispositivo == null) return NotFound();

            return View(dispositivo);
        }

        // POST: Dispositivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dispositivo = await _context.Dispositivos.FindAsync(id);
            int clienteId = dispositivo?.ClienteId ?? 0;

            if (dispositivo != null)
            {
                _context.Dispositivos.Remove(dispositivo);
                await _context.SaveChangesAsync();
            }

            if (clienteId > 0)
                return RedirectToAction("Dispositivos", "Cliente", new { id = clienteId });

            return RedirectToAction(nameof(Index));
        }

        private bool DispositivoExists(int id)
        {
            return _context.Dispositivos.Any(e => e.Id == id);
        }

        // Método auxiliar para abastecer as listas de Clientes e Produtos do Estoque
        private void CarregarListas(int? clienteId = null, int? produtoId = null)
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "RazaoSocial", clienteId);

            // Carrega produtos com estoque que não sejam de uso puramente interno
            var produtosDisponiveis = _context.Produtos
                .Where(p => p.Quantidade > 0 && !p.UsadoInternamente)
                .OrderBy(p => p.Nome)
                .Select(p => new
                {
                    p.Id,
                    DescricaoEstoque = p.Nome + " (Estoque: " + p.Quantidade + " " + (p.UnidadeMedida ?? "un") + ")"
                })
                .ToList();

            ViewBag.Produtos = new SelectList(produtosDisponiveis, "Id", "DescricaoEstoque", produtoId);
        }
    }
}

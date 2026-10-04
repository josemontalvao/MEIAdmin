using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace MEIAdmin.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Produtos (Apenas produtos ATIVOS são exibidos na tela)
        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos
                .Where(p => p.Ativo) // <-- FILTRO DE OURO: Oculta os inativos/excluídos!
                .Include(p => p.FornecedorProdutos)
                .ThenInclude(fp => fp.Fornecedor)
                .ToListAsync();

            return View(produtos);
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.FornecedorProdutos)
                .ThenInclude(fp => fp.Fornecedor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null) return NotFound();

            return View(produto);
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            ViewBag.Fornecedores = new SelectList(_context.Fornecedores, "Id", "RazaoSocial");
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Quantidade,PrecoCompra,MargemLucro,DataCompra,UnidadeMedida,Categoria,UsadoInternamente")] Produto produto, int[]? FornecedorIds)
        {
            if (ModelState.IsValid)
            {
                produto.Ativo = true; // Todo produto novo nasce ativo
                _context.Add(produto);
                await _context.SaveChangesAsync();

                if (FornecedorIds != null && FornecedorIds.Any())
                {
                    foreach (var fornecedorId in FornecedorIds)
                    {
                        _context.FornecedoresProdutos.Add(new FornecedorProduto
                        {
                            ProdutoId = produto.Id,
                            FornecedorId = fornecedorId
                        });
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Fornecedores = new SelectList(_context.Fornecedores, "Id", "RazaoSocial");
            return View(produto);
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos.Include(p => p.FornecedorProdutos)
                                                 .ThenInclude(fp => fp.Fornecedor)
                                                 .FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null) return NotFound();

            ViewBag.Fornecedores = new SelectList(_context.Fornecedores, "Id", "RazaoSocial");
            return View(produto);
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Quantidade,PrecoCompra,MargemLucro,DataCompra,UnidadeMedida,Categoria,UsadoInternamente")] Produto produto, int[]? FornecedorIds)
        {
            if (id != produto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();

                    var fornecedoresAtuais = _context.FornecedoresProdutos.Where(fp => fp.ProdutoId == produto.Id);
                    _context.FornecedoresProdutos.RemoveRange(fornecedoresAtuais);

                    if (FornecedorIds != null && FornecedorIds.Any())
                    {
                        foreach (var fornecedorId in FornecedorIds)
                        {
                            _context.FornecedoresProdutos.Add(new FornecedorProduto
                            {
                                ProdutoId = produto.Id,
                                FornecedorId = fornecedorId
                            });
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Fornecedores = new SelectList(_context.Fornecedores, "Id", "RazaoSocial");
            return View(produto);
        }

        // ==========================================
        // BAIXA DE ESTOQUE (ALMOXARIFADO)
        // ==========================================

        // GET: Produtos/Baixa/5
        public async Task<IActionResult> Baixa(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();

            return View(produto);
        }

        // POST: Produtos/Baixa/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Baixa(int id, int quantidadeBaixa, string? motivo)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();

            if (quantidadeBaixa <= 0)
            {
                ModelState.AddModelError("", "A quantidade para retirar deve ser maior que zero.");
                return View(produto);
            }

            if (quantidadeBaixa > produto.Quantidade)
            {
                ModelState.AddModelError("", $"Quantidade insuficiente! O estoque atual é de apenas {produto.Quantidade} {produto.UnidadeMedida ?? "unidades"}.");
                return View(produto);
            }

            produto.Quantidade -= quantidadeBaixa;
            _context.Update(produto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.FornecedorProdutos)
                .ThenInclude(fp => fp.Fornecedor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null) return NotFound();

            return View(produto);
        }

        // POST: Produtos/Delete/5 (EXCLUSÃO INTELIGENTE / SOFT DELETE)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                // Em vez de deletar fisicamente, marca como inativo!
                produto.Ativo = false;
                _context.Update(produto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
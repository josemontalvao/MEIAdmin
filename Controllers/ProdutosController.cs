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

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos
                .Include(p => p.FornecedorProdutos)
                .ThenInclude(fp => fp.Fornecedor)
                .ToListAsync();

            return View(produtos);
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.FornecedorProdutos)
                .ThenInclude(fp => fp.Fornecedor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

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
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.Include(p => p.FornecedorProdutos)
                                                 .ThenInclude(fp => fp.Fornecedor)
                                                 .FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            ViewBag.Fornecedores = new SelectList(_context.Fornecedores, "Id", "RazaoSocial");
            return View(produto);
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Quantidade,PrecoCompra,MargemLucro,DataCompra,UnidadeMedida,Categoria,UsadoInternamente")] Produto produto, int[]? FornecedorIds)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();

                    // Remove os fornecedores atuais do produto e adiciona os novos
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
                    if (!ProdutoExists(produto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Fornecedores = new SelectList(_context.Fornecedores, "Id", "RazaoSocial");
            return View(produto);
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.FornecedorProdutos)
                .ThenInclude(fp => fp.Fornecedor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MEIAdmin.Controllers
{
    public class FornecedoresController : Controller
    {
        private readonly AppDbContext _context;

        public FornecedoresController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var fornecedores = await _context.Fornecedores.ToListAsync();
            return View(fornecedores);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var fornecedor = await _context.Fornecedores
                .Include(f => f.FornecedorProdutos)
                .ThenInclude(fp => fp.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fornecedor == null) return NotFound();

            return View(fornecedor);
        }

        public IActionResult Create()
        {
            return View(new Fornecedor());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RazaoSocial,Contato,Telefone,Email,Endereco,Numero,Bairro,Cidade,Estado,CEP,Ativo")] Fornecedor fornecedor)
        {
            if (ModelState.IsValid)
            {
                fornecedor.Ativo = true;
                _context.Add(fornecedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fornecedor);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var fornecedor = await _context.Fornecedores
                .Include(f => f.FornecedorProdutos)
                .ThenInclude(fp => fp.Produto)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fornecedor == null) return NotFound();

            ViewData["Produtos"] = new SelectList(_context.Produtos, "Id", "Nome");
            return View(fornecedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RazaoSocial,Contato,Telefone,Email,Endereco,Numero,Bairro,Cidade,Estado,CEP,Ativo")] Fornecedor fornecedor, int[] produtosSelecionados)
        {
            if (id != fornecedor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var fornecedorAtual = await _context.Fornecedores
                    .Include(f => f.FornecedorProdutos)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (fornecedorAtual == null) return NotFound();

                // Atualiza os dados principais
                _context.Entry(fornecedorAtual).CurrentValues.SetValues(fornecedor);

                // Atualiza a relação de produtos
                fornecedorAtual.FornecedorProdutos.Clear();
                foreach (var produtoId in produtosSelecionados)
                {
                    fornecedorAtual.FornecedorProdutos.Add(new FornecedorProduto { FornecedorId = id, ProdutoId = produtoId });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Produtos"] = new SelectList(_context.Produtos, "Id", "Nome");
            return View(fornecedor);
        }

        [HttpPost]
        public async Task<IActionResult> AtivarDesativar(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null) return NotFound();

            fornecedor.Ativo = !fornecedor.Ativo;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool FornecedorExists(int id)
        {
            return _context.Fornecedores.Any(e => e.Id == id);
        }
    }
}


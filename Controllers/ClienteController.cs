using Microsoft.AspNetCore.Mvc;
using MEIAdmin.Data;
using MEIAdmin.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MEIAdmin.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes
                .Include(c => c.Dispositivos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RazaoSocial,Contato,Telefone,DataCadastro,Endereco,Numero,Bairro,Cidade,Estado,CEP,CGC,InscEstadual")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.Ativo = true; // Novo cliente sempre começa ativo
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RazaoSocial,Contato,Telefone,DataCadastro,Endereco,Numero,Bairro,Cidade,Estado,CEP,CGC,InscEstadual,Ativo")] Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Clientes.Any(e => e.Id == cliente.Id))
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
            return View(cliente);
        }

        [HttpPost]
        [HttpGet] // Permite teste via URL (ex: /Cliente/AtivarDesativar/1)
        public async Task<IActionResult> AtivarDesativar(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Ativo = !cliente.Ativo;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Dispositivos(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Dispositivos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MEIAdmin.Data;
using MEIAdmin.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

        // GET: Cliente
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Cliente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes
                .Include(c => c.Dispositivos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // GET: Cliente/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RazaoSocial,Contato,Telefone,Email,CpfCnpj,InscEstadual,DataCadastro,Endereco,Numero,Bairro,Cidade,Estado,CEP")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.Ativo = true; // Novo cliente sempre começa ativo
                if (cliente.DataCadastro == default) cliente.DataCadastro = DateTime.Today;

                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Cliente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Cliente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RazaoSocial,Contato,Telefone,Email,CpfCnpj,InscEstadual,DataCadastro,Endereco,Numero,Bairro,Cidade,Estado,CEP,Ativo")] Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            // Garante que campos opcionais em branco não travem a validação
            cliente.InscEstadual ??= string.Empty;
            cliente.Contato ??= string.Empty;
            cliente.Telefone ??= string.Empty;
            cliente.Email ??= string.Empty;
            cliente.CpfCnpj ??= string.Empty;
            cliente.Endereco ??= string.Empty;
            cliente.Numero ??= string.Empty;
            cliente.Bairro ??= string.Empty;
            cliente.Cidade ??= string.Empty;
            cliente.Estado ??= string.Empty;
            cliente.CEP ??= string.Empty;

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

        // POST/GET: Ativar ou Desativar Cliente
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> AtivarDesativar(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Ativo = !cliente.Ativo;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Cliente/Dispositivos/5
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

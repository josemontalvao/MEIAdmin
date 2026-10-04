using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace MEIAdmin.Controllers
{
    public class TecnicoController : Controller
    {
        private readonly AppDbContext _context;

        public TecnicoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Tecnico/Login
        public IActionResult Login()
        {
            var perfil = HttpContext.Session.GetString("UsuarioPerfil");
            if (perfil == "Admin") return RedirectToAction("Index", "Home");
            if (perfil == "Tecnico") return RedirectToAction(nameof(MinhasOS));

            return View();
        }

        // POST: Tecnico/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login, string senha)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Informe o usuário e a senha.";
                return View();
            }

            var colaborador = await _context.Colaboradores
                .FirstOrDefaultAsync(c => c.Login == login && c.Senha == senha && c.DataDemissao == null);

            if (colaborador == null)
            {
                ViewBag.Erro = "Usuário ou senha incorretos!";
                return View();
            }

            HttpContext.Session.SetInt32("UsuarioId", colaborador.Id);
            HttpContext.Session.SetString("UsuarioNome", colaborador.Nome ?? "Colaborador");
            HttpContext.Session.SetString("UsuarioPerfil", colaborador.Perfil ?? "Tecnico");

            // Se for Admin (José), abre a Home da empresa
            if (colaborador.Perfil == "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Se for Técnico (Alan), abre as O.S. dele
            return RedirectToAction(nameof(MinhasOS));
        }

        // GET: Tecnico/MinhasOS
        public async Task<IActionResult> MinhasOS()
        {
            var tecnicoId = HttpContext.Session.GetInt32("UsuarioId");
            if (tecnicoId == null)
            {
                return RedirectToAction(nameof(Login));
            }

            ViewBag.NomeTecnico = HttpContext.Session.GetString("UsuarioNome");

            var ordensDoTecnico = await _context.OrdensServico
                .Include(os => os.Cliente)
                .Where(os => os.ColaboradorId == tecnicoId)
                .OrderByDescending(os => os.Id)
                .ToListAsync();

            return View(ordensDoTecnico);
        }

        // GET: Tecnico/Sair
        public IActionResult Sair()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}
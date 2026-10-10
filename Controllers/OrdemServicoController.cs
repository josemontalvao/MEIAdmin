using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MEIAdmin.Data;
using MEIAdmin.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MEIAdmin.Controllers
{
    public class OrdemServicoController : Controller
    {
        private readonly AppDbContext _context;

        public OrdemServicoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrdemServico
        public async Task<IActionResult> Index()
        {
            var ordens = await _context.OrdensServico
                .Include(os => os.Cliente)
                .Include(os => os.Colaborador)
                .OrderByDescending(os => os.Id)
                .ToListAsync();

            return View(ordens);
        }

        // GET: OrdemServico/Create
        public IActionResult Create()
        {
            CarregarListas();
            var os = new OrdemServico
            {
                DataAbertura = DateTime.Today,
                Status = "Pendente"
            };
            return View(os);
        }

        // POST: OrdemServico/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataAbertura,ClienteId,ColaboradorId,TipoManutencao,DescricaoProblema,Status")] OrdemServico ordemServico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordemServico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CarregarListas(ordemServico.ClienteId, ordemServico.ColaboradorId);
            return View(ordemServico);
        }

        // GET: OrdemServico/Atender/5
        public async Task<IActionResult> Atender(int? id)
        {
            if (id == null) return NotFound();

            var os = await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Colaborador)
                .Include(o => o.Fotos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (os == null) return NotFound();

            if (!os.DataInicio.HasValue) os.DataInicio = DateTime.Now;
            if (!os.DataFim.HasValue) os.DataFim = DateTime.Now.AddHours(1);

            return View(os);
        }

        // POST: OrdemServico/Atender/5 (RECEBE LISTA DINÂMICA DE MATERIAIS)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Atender(int id, DateTime dataInicio, DateTime dataFim, string servicoExecutado, string observacoesTecnicas, List<IFormFile> fotos, List<string> legendas, string? assinaturaBase64, List<string>? matNome, List<int>? matQtd, List<string>? matUnidade)
        {
            var os = await _context.OrdensServico.Include(o => o.Fotos).FirstOrDefaultAsync(o => o.Id == id);
            if (os == null) return NotFound();

            decimal tempo = (decimal)Math.Round((dataFim - dataInicio).TotalHours, 2);
            if (tempo <= 0) tempo = 0.5m;

            os.DataInicio = dataInicio;
            os.DataFim = dataFim;
            os.TempoGastoHoras = tempo;
            os.ServicoExecutado = servicoExecutado;
            os.ObservacoesTecnicas = observacoesTecnicas;
            os.Status = "Concluída";

            // 1. Organiza a lista dinâmica de materiais e quantidades
            if (matNome != null && matNome.Count > 0)
            {
                var listaFormatada = new List<string>();
                for (int i = 0; i < matNome.Count; i++)
                {
                    string nome = matNome[i]?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(nome))
                    {
                        int qtd = (matQtd != null && i < matQtd.Count && matQtd[i] > 0) ? matQtd[i] : 1;
                        string unid = (matUnidade != null && i < matUnidade.Count) ? matUnidade[i] : "UN";
                        listaFormatada.Add($"• {qtd} {unid} - {nome}");
                    }
                }

                os.PecasUtilizadas = listaFormatada.Count > 0 
                    ? string.Join("\n", listaFormatada) 
                    : "Nenhum material registrado.";
            }
            else
            {
                os.PecasUtilizadas = "Nenhum material registrado.";
            }

            // 2. Grava a Assinatura Digital no banco
            if (!string.IsNullOrWhiteSpace(assinaturaBase64))
            {
                os.AssinaturaClienteBase64 = assinaturaBase64;
            }

            // 3. Grava as Fotos no banco
            if (fotos != null && fotos.Count > 0)
            {
                for (int i = 0; i < fotos.Count; i++)
                {
                    var file = fotos[i];
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            var bytesFoto = ms.ToArray();
                            string formato = string.IsNullOrEmpty(file.ContentType) ? "image/jpeg" : file.ContentType;
                            string fotoBase64 = $"data:{formato};base64,{Convert.ToBase64String(bytesFoto)}";

                            string legenda = (legendas != null && i < legendas.Count && !string.IsNullOrWhiteSpace(legendas[i]))
                                ? legendas[i]
                                : $"Evidência {i + 1}";

                            _context.FotosOrdemServico.Add(new FotoOrdemServico
                            {
                                OrdemServicoId = id,
                                CaminhoArquivo = fotoBase64,
                                Legenda = legenda
                            });
                        }
                    }
                }
            }

            _context.Update(os);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = os.Id });
        }

        // GET: OrdemServico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var os = await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Colaborador)
                .Include(o => o.Fotos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (os == null) return NotFound();

            return View(os);
        }

        // GET: OrdemServico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var os = await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Colaborador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (os == null) return NotFound();

            return View(os);
        }

        // POST: OrdemServico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var os = await _context.OrdensServico.Include(o => o.Fotos).FirstOrDefaultAsync(o => o.Id == id);
            if (os != null)
            {
                _context.FotosOrdemServico.RemoveRange(os.Fotos);
                _context.OrdensServico.Remove(os);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void CarregarListas(int? clienteId = null, int? colaboradorId = null)
        {
            ViewBag.Clientes = new SelectList(_context.Clientes.Where(c => c.Ativo).OrderBy(c => c.RazaoSocial), "Id", "RazaoSocial", clienteId);
            ViewBag.Colaboradores = new SelectList(_context.Colaboradores.OrderBy(c => c.Nome), "Id", "Nome", colaboradorId);
        }
    }
}

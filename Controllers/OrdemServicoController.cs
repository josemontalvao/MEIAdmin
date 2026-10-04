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

        // POST: OrdemServico/Atender/5 (RECEBE FOTOS ILIMITADAS E ASSINATURA DIGITAL)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Atender(int id, DateTime dataInicio, DateTime dataFim, string servicoExecutado, string pecasUtilizadas, string observacoesTecnicas, List<IFormFile> fotos, List<string> legendas, string? assinaturaBase64)
        {
            var os = await _context.OrdensServico.Include(o => o.Fotos).FirstOrDefaultAsync(o => o.Id == id);
            if (os == null) return NotFound();

            decimal tempo = (decimal)Math.Round((dataFim - dataInicio).TotalHours, 2);
            if (tempo <= 0) tempo = 0.5m;

            os.DataInicio = dataInicio;
            os.DataFim = dataFim;
            os.TempoGastoHoras = tempo;
            os.ServicoExecutado = servicoExecutado;
            os.PecasUtilizadas = pecasUtilizadas;
            os.ObservacoesTecnicas = observacoesTecnicas;
            os.Status = "Concluída";

            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "os");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // 1. Salva a assinatura digital desenhada com o dedo
            if (!string.IsNullOrWhiteSpace(assinaturaBase64) && assinaturaBase64.Contains(","))
            {
                try
                {
                    string base64Limpo = assinaturaBase64.Split(',')[1];
                    byte[] bytesAssinatura = Convert.FromBase64String(base64Limpo);
                    string arquivoAssinatura = Path.Combine(uploadPath, $"assinatura_{id}.png");
                    await System.IO.File.WriteAllBytesAsync(arquivoAssinatura, bytesAssinatura);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao salvar assinatura: {ex.Message}");
                }
            }

            // 2. Salva as fotos dinâmicas
            if (fotos != null && fotos.Count > 0)
            {
                for (int i = 0; i < fotos.Count; i++)
                {
                    var file = fotos[i];
                    if (file.Length > 0)
                    {
                        string ext = Path.GetExtension(file.FileName);
                        string fileName = $"OS_{id}_{Guid.NewGuid().ToString().Substring(0, 8)}{ext}";
                        string filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        string legenda = (legendas != null && i < legendas.Count && !string.IsNullOrWhiteSpace(legendas[i]))
                            ? legendas[i]
                            : $"Evidência {i + 1}";

                        _context.FotosOrdemServico.Add(new FotoOrdemServico
                        {
                            OrdemServicoId = id,
                            CaminhoArquivo = $"/uploads/os/{fileName}",
                            Legenda = legenda
                        });
                    }
                }
            }

            _context.Update(os);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = os.Id });
        }

        // GET: OrdemServico/Details/5 (Visualização do Laudo)
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

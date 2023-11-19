using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DespesasCartao.Data;
using DespesasCartao.Models;

namespace DespesasCartao.Pages.PageProduto
{
    public class DeleteModel : PageModel
    {
        private readonly DespesasCartao.Data.DespesasCartaoContext _context;
        private readonly IWebHostEnvironment _environment;

        public DeleteModel(DespesasCartao.Data.DespesasCartaoContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
      public Produto Produto { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Produto == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto.FirstOrDefaultAsync(m => m.IdProduto == id);

            if (produto == null)
            {
                return NotFound();
            }
            else 
            {
                Produto = produto;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Produto == null)
            {
                return NotFound();
            }
            var produto = await _context.Produto.FindAsync(id);

            if (produto != null)
            {
                Produto = produto;
                _context.Produto.Remove(Produto);
               if(await _context.SaveChangesAsync() > 0)
                {
                    var caminhoArquivoImagem = Path.Combine(
                        _environment.WebRootPath, "img\\produto", Produto.IdProduto.ToString("D6") + ".jpg");
                    if(System.IO.File.Exists(caminhoArquivoImagem))
                    {
                        System.IO.File.Delete(caminhoArquivoImagem);
                    }
                }
            }

            return RedirectToPage("./Index");
        }
    }
}

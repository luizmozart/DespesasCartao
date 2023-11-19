using DespesasCartao.Data;
using DespesasCartao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DespesasCartao.Pages.PageProduto
{
    public class EditModel : PageModel
    {
        private readonly DespesasCartaoContext _context;
        private readonly IWebHostEnvironment _environment;

        

        public EditModel(DespesasCartao.Data.DespesasCartaoContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Produto Produto { get; set; } = default!;
        public string caminhoImagem { get; set; }

        [BindProperty]
        [Display(Name ="Imagem do Produto")]
        public IFormFile ImagemProduto { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Produto == null)
            {
                return NotFound();
            }

            var produto =  await _context.Produto.FirstOrDefaultAsync(m => m.IdProduto == id);
            if (produto == null)
            {
                return NotFound();
            }

            if(caminhoImagem != null)
            {
                caminhoImagem = $"~/img/produto/{Produto.IdProduto:D6}.jpg";
            }

            
            Produto = produto;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                if(ImagemProduto != null)
                {
                    await AppUtils.ProcessarArquivoDeImagem(Produto.IdProduto, ImagemProduto, _environment);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(Produto.IdProduto))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProdutoExists(int id)
        {
          return (_context.Produto?.Any(e => e.IdProduto == id)).GetValueOrDefault();
        }
    }
}

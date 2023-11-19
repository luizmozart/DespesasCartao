using DespesasCartao.Data;
using DespesasCartao.Models;
using DespesasCartao.Pages.PageCliente;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace DespesasCartao.Pages.PageProduto
{
    public class CreateModel : PageModel
    {
        private readonly DespesasCartao.Data.DespesasCartaoContext _context;
        private readonly IWebHostEnvironment _environment;
                        
        public CreateModel(DespesasCartaoContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            caminhoImagem = "~/img/produto/sem_imagem.jpg";
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Produto Produto { get; set; } = default!;

        public string caminhoImagem { get; set; }

        [BindProperty]
        [Display(Name = "Imagem do Produto")]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage ="O campo \"{0}\" é de preenchimento obrigatório")]
        public IFormFile ImagemProduto { get; set; }

        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if(ImagemProduto == null)
            {
                return Page();
            }

            var produto = new Produto();

            if(await TryUpdateModelAsync(produto, Produto.GetType(), nameof(Produto)))
            {
                _context.Produto.Add(produto);
                await _context.SaveChangesAsync();
                await AppUtils.ProcessarArquivoDeImagem(Produto.IdProduto, ImagemProduto, _environment);

                return RedirectToPage("./Listar");
            }

            return Page();
          //if (!ModelState.IsValid || _context.Produto == null || Produto == null)
          //  {
          //      return Page();
          //  }

          //  _context.Produto.Add(Produto);
          //  await _context.SaveChangesAsync();

          //  return RedirectToPage("./Index");
        }
    }
}

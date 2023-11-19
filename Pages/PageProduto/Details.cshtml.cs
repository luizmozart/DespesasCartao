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
    public class DetailsModel : PageModel
    {
        private readonly DespesasCartao.Data.DespesasCartaoContext _context;

        public DetailsModel(DespesasCartao.Data.DespesasCartaoContext context)
        {
            _context = context;
        }

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
    }
}

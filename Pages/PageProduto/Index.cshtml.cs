using DespesasCartao.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DespesasCartao.Pages.PageProduto
{
    public class IndexModel : PageModel
    {
        private readonly DespesasCartao.Data.DespesasCartaoContext _context;

        public IndexModel(DespesasCartao.Data.DespesasCartaoContext context)
        {
            _context = context;
        }

        public IList<Produto> Produto { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Produto != null)
            {
                Produto = await _context.Produto.ToListAsync();
            }
        }
    }
}

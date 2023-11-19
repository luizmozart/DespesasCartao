using DespesasCartao.Data;
using DespesasCartao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DespesasCartao.Pages
{
    public class IndexModel : PageModel
    {
        private const int tamanhoPagina = 4;
        private readonly ILogger<IndexModel> _logger;
        private DespesasCartaoContext _context;
        public int PaginaAtual { get; set; }
        public int QuantidadePaginas { get; set; }

        public IList<Produto> Produtos;

        public IndexModel(ILogger<IndexModel> logger, DespesasCartaoContext context)
        {
            _logger = logger;
            _context = context; 
        }

        public async Task OnGetAsync([FromQuery(Name ="q")]string termobusca,
            [FromQuery(Name ="o")] int? ordem = 1, 
            [FromQuery(Name ="p")] int? pagina = 1)
        {
            this.PaginaAtual = pagina.Value;

            var query = _context.Produto.AsQueryable();

            if(!string.IsNullOrEmpty(termobusca))
            {
                query = query.Where(p => p.Nome.ToUpper().Contains(termobusca.ToUpper()));
                
            }
            if(ordem.HasValue)
            {
                switch(ordem.Value)
                {
                    case 1:
                        query = query.OrderBy(p => p.Nome.ToUpper());
                        break;

                    case 2:
                        query = query.OrderBy(p => p.Preco);
                        break;

                    case 3:
                        query = query.OrderByDescending(p => p.Preco);
                        break;

                }
            }

            var queryCount = query;
            int qtdeProdutos = queryCount.Count();
            this.QuantidadePaginas = Convert.ToInt32(Math.Ceiling(qtdeProdutos * 1M / tamanhoPagina));
            query = query.Skip(tamanhoPagina * (this.PaginaAtual - 1)).Take(tamanhoPagina);

            Produtos = await query.ToListAsync();
            
        }
    }
}
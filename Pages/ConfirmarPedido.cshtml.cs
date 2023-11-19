using DespesasCartao.Data;
using DespesasCartao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DespesasCartao.Pages
{
    [Authorize(Roles ="cliente")]
    public class ConfirmarPedidoModel : PageModel
    {
        private readonly DespesasCartaoContext _context;
        public string COOKIE_NAME
        {
            get { return ".AspNetCore.CardId"; }
        }
        public Pedido pedido { get; set; }

        public Cliente cliente { get; set; }

        public ConfirmarPedidoModel(DespesasCartaoContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (Request.Cookies.ContainsKey(COOKIE_NAME))
            {
                var cartId = Request.Cookies[COOKIE_NAME];

                pedido = await _context.Pedidos.Include(p => p.ItensPedido).ThenInclude(ip => ip.IdProduto)
                    .FirstOrDefaultAsync(p => p.IdCarrinho == cartId);
                if(pedido != null)
                {
                    cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);
                    pedido.IdCliente = cliente.IdCliente;
                    pedido.Endereco = cliente.Endereco;
                    pedido.ValorTotal = pedido.ItensPedido.Sum(x => x.Quantidade * x.ValorUnitario);
                    await _context.SaveChangesAsync();
                }
            }

            return Page();

        }

        
    }
}

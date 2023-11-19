using DespesasCartao.Data;
using DespesasCartao.Models;
using DespesasCartao.Servico;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DespesasCartao.Pages
{
    [Authorize(Roles = "cliente")]
    public class FinalizarPedidoModel : PageModel
    {
        private readonly DespesasCartaoContext _context;
        private IEmailSender _emailSender;
        public string COOKIE_NAME
        {
            get { return ".AspNetCore.CardId"; }
        }
        public Pedido pedido { get; set; }

        public Cliente cliente { get; set; }

        public FinalizarPedidoModel(DespesasCartaoContext context,
                                    IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (Request.Cookies.ContainsKey(COOKIE_NAME))
            {
                var cartId = Request.Cookies[COOKIE_NAME];

                pedido = await _context.Pedidos
                    .Include(p => p.ItensPedido)
                    .ThenInclude(ip => ip.IdProduto)
                    .FirstOrDefaultAsync(p => p.IdCarrinho == cartId);

                cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);

                if((pedido.IdCliente > 0) && (pedido.Endereco != null))
                {
                    pedido.Situacao = Pedido.SituacaoPedido.Realilzado;
                    pedido.DataHoraPedido = DateTime.UtcNow;
                    foreach(var item in pedido.ItensPedido)
                    {
                        item.Produto.Estoque -= (int)item.Quantidade;
                    }

                    await _context.SaveChangesAsync();
                    Response.Cookies.Delete(COOKIE_NAME);
                    return Page();
                }

                
            }
            else
            {
                return RedirectToPage("/ConfirmarPedido");
            }

            return RedirectToPage("/carrinho");
        }
    }
}

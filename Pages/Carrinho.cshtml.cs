using DespesasCartao.Data;
using DespesasCartao.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Transactions;

namespace DespesasCartao.Pages
{
    public class CarrinhoModel : PageModel
    {
        private readonly DespesasCartaoContext _context;
        private readonly SignInManager<AppUser> _singInManager;
        private readonly UserManager<AppUser> _userManager;
        public string COOKIE_NAME
        {
            get { return ".AspNetCore.CardId"; }
        }


        public Pedido pedido { get; set; }
        public double TotalPedido { get; set; }

        public CarrinhoModel(DespesasCartaoContext context,
                             SignInManager<AppUser> singInManager,
                             UserManager<AppUser> userManager)
        {
            _context = context;
            _singInManager = singInManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (Request.Cookies.ContainsKey(COOKIE_NAME))
            {
                var cardId = Request.Cookies[COOKIE_NAME];
                pedido = await _context.Pedidos.Include("ItensPedido").Include("ItensPedido.Produto")
                    .FirstOrDefaultAsync(p => p.IdCarrinho == cardId);
                if (pedido != null)
                {
                    TotalPedido = pedido.ItensPedido.Sum(x => x.Quantidade * Convert.ToDouble(x.ValorUnitario));
                }
                else
                {
                    TotalPedido = 0;
                }
            }
            else
            {
                SetCartCookie();

            }

            return Page();
        }

        public string SetCartCookie()
        {
            var cartId = Guid.NewGuid().ToString();
            var options = new Microsoft.AspNetCore.Http.CookieOptions()
            {
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(90),
                IsEssential = true,
                Secure = false,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                HttpOnly = false
            };

            Response.Cookies.Append(COOKIE_NAME, cartId, options);

            return cartId;
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int? id, int qtde = 1)
        {
            if(id == null) return NotFound();

            var produto = await _context.Produto.FindAsync(id);

            if(produto != null)
            {
                string cartId = null;
                if(Request.Cookies.ContainsKey(COOKIE_NAME))
                {
                    cartId = Request.Cookies[COOKIE_NAME];
                    pedido = await _context.Pedidos.Include("ItensPedido").Include("ItensPedido.Produto").FirstOrDefaultAsync( p => p.IdCarrinho == cartId);
                }
                else
                {
                    cartId = SetCartCookie();
                }

                if(pedido == null)
                {
                    pedido = new Pedido
                    {
                        IdCarrinho = cartId,
                        DataHoraPedido = DateTime.UtcNow,
                        Situacao = Pedido.SituacaoPedido.carrinho,
                        ItensPedido = new List<ItemPedido>()
                    };

                    AppUser appUser = _singInManager.IsSignedIn(User) ? await _userManager.GetUserAsync(User) : null;

                    if(appUser != null)
                    {
                        Cliente cliente = await _context.Clientes.FirstOrDefaultAsync<Cliente>(
                            c => c.Email.ToLower().Equals(appUser.Email.ToLower()));

                        if(cliente != null)
                        {
                            pedido.IdCliente = cliente.IdCliente;
                        }
                    }
                    _context.Pedidos.Add(pedido);
                }

                var itemPedido = pedido.ItensPedido.FirstOrDefault(ip => ip.IdPedido == id);
                if(itemPedido == null)
                {
                    pedido.ItensPedido.Add(new ItemPedido
                    {
                        IdProduto = id.Value,
                        Quantidade = qtde,
                        ValorUnitario = produto.Preco.Value
                    });
                }
                else
                {
                    itemPedido.Quantidade += qtde;
                }

                if(_context.SaveChanges() <= 0)
                {
                    ModelState.AddModelError("", "Ocorreu um erro ao adicionar o item no carrinho.");
                }
            }

            TotalPedido = pedido.ItensPedido.Sum(x => x.Quantidade * x.ValorUnitario);

            return Page();
        }

    }
}

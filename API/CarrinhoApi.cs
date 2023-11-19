﻿using DespesasCartao.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DespesasCartao.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CarrinhoApi : ControllerBase
    {
        private DespesasCartaoContext _context;

        public CarrinhoApi(DespesasCartaoContext context)
        {
            _context = context;        
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCartItem([FromForm] string idCarrinho, [FromForm] int? idProduto = 0, [FromForm] int? quantidade = 0)
        {
            if (string.IsNullOrEmpty(idCarrinho) || (idProduto <= 0) || (quantidade <= 0))
                return new JsonResult(false); 

            var pedido = await _context.Pedidos.Include("ItensPedido").FirstOrDefaultAsync(p => p.IdCarrinho == idCarrinho);
            if(pedido != null)
            {
                if(pedido.Situacao == Models.Pedido.SituacaoPedido.carrinho)
                {
                    var itemPedido = pedido.ItensPedido.FirstOrDefault(ip => ip.IdProduto == idProduto);
                    if(itemPedido != null)
                    {
                        itemPedido.Quantidade = quantidade.Value;

                        if(_context.SaveChanges() > 0)
                        {
                            double valorPedido = pedido.ItensPedido.Sum(ip => ip.ValorItem);
                            var item = pedido.ItensPedido.Select(
                                x => new {id = x.IdProduto, q = x.Quantidade, v = x.ValorItem})
                                .FirstOrDefault(ip => ip.id == idProduto);
                            var jsonRes = new JsonResult(new { v = valorPedido, item });
                            return jsonRes;
                        }
                    }
                }

            }

            return new JsonResult(false);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteCartItem([FromForm] string idCarrinho, [FromForm] int? idProduto = 0)
        {
            if (string.IsNullOrEmpty(idCarrinho) || (idProduto <= 0)) return new JsonResult(false);

            var pedido = await _context.Pedidos.Include("ItensPedido").FirstOrDefaultAsync(p => p.IdCarrinho == idCarrinho);
            if(pedido != null)
            {
                if (pedido.Situacao == Models.Pedido.SituacaoPedido.carrinho)
                {
                    var itemPedido = pedido.ItensPedido.FirstOrDefault(ip => ip.IdProduto == idProduto);
                    if(itemPedido != null)
                    {
                        pedido.ItensPedido.Remove(itemPedido);

                        if(_context.SaveChanges() > 0)
                        {
                            double valorPedido = pedido.ItensPedido.Sum(ip => ip.ValorItem);
                            var jsonres = new JsonResult(new { v = valorPedido, id = idProduto });
                            return jsonres;
                        }
                    }
                }
            }

            return new JsonResult(false);
        }
    }
}

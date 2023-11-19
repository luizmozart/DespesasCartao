using DespesasCartao.Data;
using DespesasCartao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DespesasCartao.Pages.PageCliente
{
    //[Authorize(Policy = "isAdmin")]
    public class IncluirModel : PageModel
    {
        private readonly DespesasCartaoContext _context;

        [BindProperty]
        public Cliente Cliente { get; set; }

        public IncluirModel(DespesasCartaoContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cliente = new Cliente();
            cliente.Endereco = new Endereco();
            cliente.Situacao = Cliente.SituacaoCliente.Cadastrado;

            if (await TryUpdateModelAsync(cliente, Cliente.GetType(), nameof(Cliente)))
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToPage("./listar");
            }

            //if (await TryUpdateModelAsync<Cliente>(cliente, "cliente", obj => obj.Nome, obj => obj.DataNascimento, obj => obj.CPF, obj => obj.Email ))
            //{
            //    _context.Clientes.Add(cliente);
            //    await _context.SaveChangesAsync();
            //    return RedirectToPage("./listar");
            //}

            return Page();
        }
    }
}

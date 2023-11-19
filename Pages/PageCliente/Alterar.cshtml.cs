using DespesasCartao.Data;
using DespesasCartao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DespesasCartao.Pages.PageCliente
{
    //[Authorize(Policy = "isAdmin")]
    public class AlterarModel : PageModel
    {
        private readonly DespesasCartaoContext _context;

        [BindProperty]
        public Cliente Cliente { get; set; }

        public AlterarModel(DespesasCartaoContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            Cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.IdCliente == Id);
            if (Cliente == null)
            {
                return NotFound();
            }

            return Page();


        }

        public async Task<IActionResult> OnPostAsync()
        {
            //para garantir que o CEP e o Email nao serao atualizados
            var cliente = await _context.Clientes.Select(m => new { m.IdCliente, m.Email, m.CPF }).FirstOrDefaultAsync();
            Cliente.Email = cliente.Email;
            Cliente.CPF = cliente.CPF;

            if(ModelState.Keys.Contains("Cliente.Email"))
            {
                ModelState["Cliente.Email"].Errors.Clear();
                ModelState.Remove("Cliente.Email");
            }
            if (ModelState.Keys.Contains("Cliente.CPF"))
            {
                ModelState["Cliente.CPF"].Errors.Clear();
                ModelState.Remove("Cliente.CPF");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Cliente).State = EntityState.Modified;
            _context.Attach(Cliente.Endereco).State = EntityState.Modified;

            try
            {
                _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) 
            {
                if(!ClienteAindaExiste(Cliente.IdCliente))
                {
                    return NotFound();  
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./listar");
        }

        private bool ClienteAindaExiste(int Id)
        {
            return _context.Clientes.Any(c => c.IdCliente == Id);
        }
    }
}

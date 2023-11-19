using DespesasCartao.Data;
using DespesasCartao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DespesasCartao.Pages.PageCliente
{
    //[Authorize(Policy = "isAdmin")]
    public class ListarModel : PageModel
    {
        private readonly DespesasCartaoContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IList<string> EmailsAdmins { get; set; }

        public IList<Cliente> clientes { get; set; }

        public ListarModel(DespesasCartaoContext context,
                           UserManager<AppUser> userManager,
                           RoleManager<IdentityRole> roleManager)
        {
            this._context = context;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        public async Task OnGetAsync()
        {
            EmailsAdmins = (await _userManager.GetUsersInRoleAsync("admin"))
                .Select(x => x.Email).ToList();
            clientes = await _context.Clientes.ToListAsync();
        }

        public async Task<ActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                if (await _context.SaveChangesAsync() > 0)
                {
                    AppUser usuario = await _userManager.FindByNameAsync(cliente.Email);
                    if (usuario != null) 
                    {
                        await _userManager.DeleteAsync(usuario);
                    }
                }
            }

            return RedirectToPage("./listar");
        }

        public async Task<ActionResult> OnPostDelAdminAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente != null)
            {
                var usuario = await _userManager.FindByEmailAsync(cliente.Email);
                if (usuario != null) 
                {
                    await _userManager.RemoveFromRoleAsync(usuario, "Admin");     
                }
            }

            return RedirectToPage("./listar");
        }

        public async Task<ActionResult> OnPostSetAdminAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente != null)
            {
                AppUser usuario = await _userManager.FindByEmailAsync(cliente.Email);
                if (usuario != null)
                {
                    if (!await _roleManager.RoleExistsAsync("admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                    }
                    await _userManager.AddToRoleAsync(usuario, "admin");
                }
            }

            return RedirectToPage("./listar");
        }

    }
}

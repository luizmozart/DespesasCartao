using DespesasCartao.Data;
using DespesasCartao.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DespesasCartao.Pages
{
    public class NovoUsuarioModel : PageModel
    {
        public class Senhas
        {
            [Required(ErrorMessage ="O campo \"{0}\" é de preenchimento obrigatório")]
            [StringLength(16,ErrorMessage = "O campo \"{0}\" deve ter pelo menos {2} e no mínimo {1} caracteres", MinimumLength =6)]
            [DataType(DataType.Password)]
            [Display(Name ="Senha")]
            public string Senha { get; set; }

            [Required(ErrorMessage = "O campo \"{0}\" é de preenchimento obrigatório")]
            [StringLength(16, ErrorMessage = "O campo \"{0}\" deve ter pelo menos {2} e no mínimo {1} caracteres")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmação Senha")]
            [Compare("Senha", ErrorMessage ="A confirmação da senha não confere com a senha informada.")]
            public string ConfirmacaoSenha { get; set; }
        }
        private readonly DespesasCartaoContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public NovoUsuarioModel(DespesasCartaoContext context, 
                                UserManager<AppUser> userManager, 
                                RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public Cliente Cliente { get; set; }

        [BindProperty]
        public Senhas SenhasUsuario { get; set; }


        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cliente = new Cliente();
            cliente.Endereco = new Endereco();

            cliente.Situacao = Cliente.SituacaoCliente.Cadastrado;

            var senhasUsuario = new Senhas();
            if (!await TryUpdateModelAsync(senhasUsuario, senhasUsuario.GetType(), nameof(senhasUsuario)))
                

            if (!await TryUpdateModelAsync(cliente ,Cliente.GetType(), nameof(Cliente)))
            {
                if(! await _roleManager.RoleExistsAsync("cliente"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("cliente"));
                }
                
                var usuarioExsitente = await _userManager.FindByEmailAsync(cliente.Email);
                if(usuarioExsitente !=  null)
                {
                    ModelState.AddModelError("Cliente.Email", "Já existe um cliente cadastrado com esse e-mail, informe outro.");
                    return Page();
                }

                var usuario = new AppUser()
                {
                    UserName = cliente.Email,
                    Email = cliente.Email,
                    PhoneNumber = cliente.Telefone,
                    Nome = cliente.Nome
                };

                var result = await _userManager.CreateAsync(usuario, senhasUsuario.Senha);

                if(result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(usuario, "cliente");

                    _context.Clientes.Add(cliente);
                    int afetados = await _context.SaveChangesAsync();
                    if(afetados > 0)
                    {
                            //https://kenhaggerty.com/articles/aspnet-core-31-smpt-emailsender
                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
                        return RedirectToPage("/CadastroRealizado");
                    }
                    else
                    {
                        await _userManager.DeleteAsync(usuario);
                        ModelState.AddModelError("Cliente", "Não foi possível efetuar o cadastro. Verifique e tente novamente");
                        return Page();
                    }

                }
                else
                {
                    ModelState.AddModelError("Cliente.Email", "Não foi possível efetuar o cadastro. Verifique e tente novamente");
                }
            }

            return Page();
        }
    }
}

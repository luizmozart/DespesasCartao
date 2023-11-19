using DespesasCartao.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;

namespace DespesasCartao.Pages
{
    public class LoginModel : PageModel
    {
        public class DadosLogin
        {
            [Required(ErrorMessage = "O campo \"{0}\" é de preenchimento obrigatório")]
            [EmailAddress]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "E-Mail")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "O campo \"{0}\" é de preenchimento obrigatório")]
            [StringLength(16, ErrorMessage = "O campo \"{0}\" deve ter pelo menos {2} e no mínimo {1} caracteres", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string? Senha { get; set; }

            [Display(Name ="Lembrar")]
            public string Lembrar { get; set; }
        }

        private readonly SignInManager<AppUser> _signInManager;

        public LoginModel(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public DadosLogin Dados { get; set; }
        public string ReturnUrl { get; set; }
        //https://www.learnrazorpages.com/razor-pages/tempdata
        [TempData]
        public string MensagemErro { get; set; }
        public async Task OnGetAsync(string returnUrl = null)
        {
            if(!String.IsNullOrEmpty(MensagemErro)) 
            {
                ModelState.AddModelError(String.Empty, MensagemErro);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            //elimina cookie anterior para garantir o processo de login
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var result = await _signInManager.PasswordSignInAsync(Dados.Email,Dados.Senha,false, lockoutOnFailure:false);
            if(result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "tentativa de Login inválida");
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if(returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}

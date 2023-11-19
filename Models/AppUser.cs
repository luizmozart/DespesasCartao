using Microsoft.AspNetCore.Identity;


namespace DespesasCartao.Models
{
    public class AppUser : IdentityUser
    {
        public string Nome { get; set; }
    }
}

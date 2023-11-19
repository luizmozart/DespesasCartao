using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DespesasCartao.Models
{
    public class Cliente
    {
        public enum SituacaoCliente
        {
            Bloqueado,
            Cadastrado,
            Aprovado,
            Especial
        }
        [Key]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "O campo \"{0}\" é de preenchimento obrigatório !!")]
        [MaxLength(100,ErrorMessage = "O campo \"{0}\" deve ter no máximo {1} caracter ")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo \"{0}\" é de preenchimento obrigatório !!")]
        [DisplayName("Data de Nascimento")]
        [DataType(DataType.Date,ErrorMessage = "O campo \"{0}\" deve ter uma data válida")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O campo \"{0}\" é de preenchimento obrigatório !!")]
        [MaxLength(11,ErrorMessage = "O campo \"{0}\" deve ter {1} caracteres")]
        [MinLength(11, ErrorMessage = "O campo \"{0}\" deve ter {1} caracteres")]
        [RegularExpression(@"[0-9]{11}$", ErrorMessage = "O campo \"{0}\" deve ter {1} caracteres")]
        [UIHint("_customCPF")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O campo \"{0}\" é de preenchimento obrigatório !!")]
        [MaxLength(11, ErrorMessage = "O campo \"{0}\" deve ter {1} caracteres")]
        [MinLength(11, ErrorMessage = "O campo \"{0}\" deve ter {1} caracteres")]
        [RegularExpression(@"[0-9]{11}$", ErrorMessage = "O campo \"{0}\" deve ter {1} caracteres")]
        [UIHint("_customTelefone")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O campo \"{0}\" é de preenchimento obrigatório !!")]
        [DisplayName("E-Mail")]
        [EmailAddress(ErrorMessage = "O campo \"{0}\" deve ter um endereço de email válido.")]
        [MaxLength(50, ErrorMessage = "O campo \"{0}\" deve ter um endereço de email válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo \"{0}\" é de preenchimento obrigatório !!")]
        [DisplayName("Situação")]
        public SituacaoCliente Situacao { get; set; }

        public Endereco Endereco { get; set; }

        public ICollection<Pedido> Pedidos { get; set; }
    }
}

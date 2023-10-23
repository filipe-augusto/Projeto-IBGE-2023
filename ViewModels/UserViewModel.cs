using System.ComponentModel.DataAnnotations;

namespace ProjetoIBGE.ViewModels
{
    public class UserViewModel
    {

        [Required(ErrorMessage ="Nome necessario")]
        [MinLength(5, ErrorMessage = "Nome aceita no minimo 5 caracateres")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Senha necessaria")]
        [MinLength(6, ErrorMessage = "Senha aceita no minimo 6 caracateres")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Confirmação da senha necessaria")]
        [MinLength(6, ErrorMessage = "Senha aceita no minimo 6 caracateres")]
        public required string PasswordConfirmation { get; set; }

    }
}

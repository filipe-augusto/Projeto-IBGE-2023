using System.ComponentModel.DataAnnotations;

namespace ProjetoIBGE.ViewModels
{
    public class LoginViewModel
    {
        [MaxLength(50, ErrorMessage = "No maximo 50 caracteres para o login")]
        [Required(ErrorMessage = "Nome necessario")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Senha necessaria")]
        [MaxLength(50, ErrorMessage = "No maximo 50 caracteres para a senha")]
        public string PassWord { get; set; }
    }
}

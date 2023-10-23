using System.ComponentModel.DataAnnotations;

namespace ProjetoIBGE.Models
{
    public class Cidade
    {
        [Required(ErrorMessage = "Codigo da cidade é necessario")] 
        [MaxLength(7, ErrorMessage = "Digite 7 caracateres para o codigo da cidade")]
        [MinLength(7, ErrorMessage = "Digite 7 caracateres para o codigo da cidade")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Estado é necessario")]
        [MaxLength(2, ErrorMessage = "Digite no maximo 2 caracateres para o estado")]
        [MinLength(2, ErrorMessage = "Digite no minimo 2 caracateres para o estado")]
        public string State { get; set; }


        [Required(ErrorMessage = "Cidade é necessario")]
        [MaxLength(80, ErrorMessage ="Digite no maximo 80 caracateres para a cidade")]
        [MinLength(2, ErrorMessage = "Digite no minimo 2 caracateres para a cidade")]
        public string City { get; set; }

    }
}

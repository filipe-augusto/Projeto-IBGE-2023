using System.ComponentModel.DataAnnotations;

namespace ProjetoIBGE.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Nome Necessario")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Senha Necessaria")]
        public string PassWordHash { get; set; }
        public bool IsActive { get; set; }

        public DateTime DateCreate { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestAlchemy.Models
{
    [Table("Genero")]
    public class Genero
    {
        [Key]
        public int GenreId { get; set; }
        
        [Required(ErrorMessage = "el campo es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Image { get; set; }


      
    }
}

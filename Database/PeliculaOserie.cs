using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestAlchemy.Models
{
    [Table("PeliculaOserie")]
    public class PeliculaOserie
    {

        [Key]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Imagen { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public DateTime FechaDeCreacion { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public int Calificacion { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string PersonajesAsociados { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public int GenreId { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genero Genero { get; set; }

    }
}

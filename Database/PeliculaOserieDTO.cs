using System.ComponentModel.DataAnnotations;


namespace ApiRestAlchemy.Models
{
    public class PeliculaOserieDTO
    {
        [Key]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Imagen { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public DateTime FechaDeCreacion { get; set; }


    }
}

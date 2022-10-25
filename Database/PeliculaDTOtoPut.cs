using System.ComponentModel.DataAnnotations;


namespace ApiRestAlchemy.Models
{
    public class PeliculaDTOtoPut
    {

    

        [Required(ErrorMessage = "el campo es requerido")]
        public string Titulo { get; set; }

    

        [Required(ErrorMessage = "el campo es requerido")]
        public DateTime FechaDeCreacion { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public int Calificacion { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string PersonajesAsociados { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public int GenreId { get; set; }



    }
}

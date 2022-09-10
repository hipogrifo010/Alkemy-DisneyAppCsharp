using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRestAlchemy.Models
{
    [Table("Personaje")]
    public class Personaje
    {
        [Key]
        public int CharacterId { get; set; }
        [Required(ErrorMessage ="el campo es requerido")]
        public string Nombre { get; set; }
     
        [Required(ErrorMessage = "el campo es requerido")]
        public string Imagen { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Peso { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Historia { get; set; }
       
        public int MovieId {get;set;}

        [ForeignKey("MovieId")]
        public virtual PeliculaOserie PeliculaOserie { get; set; }
        

    }
}

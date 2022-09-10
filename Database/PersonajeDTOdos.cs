using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRestAlchemy.Models
{
    public class PersonajeDTOdos
    {
        [Key]
        public int CharacterId { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Imagen { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Peso { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Historia { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public int MovieId { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRestAlchemy.Models
{
    public class PersonajeDTO
    {
        

        [Required(ErrorMessage = "el campo es requerido")]
        public int CharacterId { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "el campo es requerido")]
        public string Imagen { get; set; }
        
    }
}

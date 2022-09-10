using Microsoft.AspNetCore.Mvc;
using ApiRestAlchemy.Database;
using ApiRestAlchemy.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Principal;

namespace ApiRestAlchemy.Controllers
{


   [Route("api/[controller]")]
   [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]

    [ApiController]
    public class GeneralController : ControllerBase
    {

        private DatabaseContext _context;

        public GeneralController(DatabaseContext context)
        {
            _context = context;
        }


        /////////////Personaje///////////
  
        /// <LISTADOCHARACTERS>
        /// https://localhost:7105/Listado/characters
        /// </Retorna Listado de personajes>
        
        [HttpGet("/Listado/characters")]
        public async Task<ActionResult<IEnumerable<PersonajeDTO>>> ListadoPersonajes()
        {
            return await _context.Personajes
                .Select(x => PersonajeToDTO(x))
                .ToListAsync();
        }



        /// <DETALLLECHARACTER>
        /// Utilizar nombre luego  del endpoint  Eje : "https://localhost:7105/DetalleCharacter/Woody"
        /// https://localhost:7105/DetalleCharacter/
        /// </Retorna un personaje con el correspondiente Titulo de la pelicula de la participa>

        [HttpGet("/DetalleCharacter/{CharacterName}")]
        public ActionResult DetalleCharacter(string CharacterName)
        {


            int peliculaId = _context.Personajes.Where(x => x.Nombre.Equals(CharacterName))
                                               .Select(x => x.MovieId).FirstOrDefault();


            var personajePelicula = _context.Personajes.Join(_context.PeliculasOseries, personaje => personaje.MovieId,
                                     pelicula => pelicula.MovieId, (personaje, pelicula) => new { pelicula, personaje })
                                    .Where(x => x.personaje.MovieId == peliculaId);


            return Ok(personajePelicula
                                       .Where(x => x.personaje.Nombre.Equals(CharacterName))
                                       .Select(x => new {
                                           x.personaje.Nombre,
                                           x.personaje.Edad,
                                           x.personaje.Imagen,
                                           x.personaje.CharacterId,
                                           x.personaje.Historia,
                                           x.personaje.Peso,
                                           x.personaje.MovieId,
                                           x.personaje.PeliculaOserie.Titulo,
                                       }));

        }



        /// <SEARCHCHARACTERS>
        /// Utilizar Query a partir de las siguientes URL
        /// https://localhost:7105/Busqueda/Characters?name="NOMBRE"
        /// https://localhost:7105/Busqueda/Characters?age="EDAD"
        /// https://localhost:7105/Busqueda/Characters?movieId="MovieId"
        /// </Retorna Query de busqueda de Characters>
        [HttpGet("/Busqueda/Characters")]
        public async Task<ActionResult<List<PersonajeDTO>>> SearchCharacters([FromQuery] string? name, int? age, int? movies)
        {

            var personajeQueryable = _context.Personajes.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                personajeQueryable = personajeQueryable.Where(x => x.Nombre.Contains(name));
            }
            if (age != null)
            {
                personajeQueryable = personajeQueryable.Where(x => x.Edad.Equals(age));
            }

            if (movies != null)
            {
                personajeQueryable = personajeQueryable.Where(x => x.MovieId.Equals(movies));

            }


            return await personajeQueryable
                .Select(x => PersonajeToDTO(x))
                .ToListAsync();
        }

        
        
        /// <POSTCHARACTERS>
        /// 
        /// </ADVERTENCIA!,CharacterId es identidad ,es decir dejar en Valor 0>

        [HttpPost("/Listado/Post/characters")]
        public async Task<ActionResult<Personaje>> PostCharacter([FromBody] PersonajeDTOdos personajeDTOdos)
        {
           Personaje persona = new()
            {

               CharacterId = personajeDTOdos.CharacterId,
               Nombre = personajeDTOdos.Nombre,
               Imagen = personajeDTOdos.Imagen,
               Edad = personajeDTOdos.Edad,
               Peso = personajeDTOdos.Peso,
               Historia = personajeDTOdos.Historia,
               MovieId = personajeDTOdos.MovieId
           };
           _context.Personajes.Add(persona);
            await _context.SaveChangesAsync();

            return CreatedAtAction("ListadoPersonajes", new { id =persona.CharacterId }, persona);

        }

 

        /// <PUTCHARACTERS>
        /// 
        /// </ingresar id del Personaje como Value,y tambien dentro del BODY>
        [HttpPut("/Listado/characters/{id}")]
        public async Task<ActionResult<Personaje>> CharacterModification(int id, PersonajeDTOdos characterput)
        {

            Personaje personaje = new()
            {
                CharacterId = characterput.CharacterId,
                Nombre = characterput.Nombre,
                Imagen = characterput.Imagen,
                Edad = characterput.Edad,
                Peso = characterput.Peso,
                Historia = characterput.Historia,
                MovieId = characterput.MovieId
            };

            if (id != personaje.CharacterId)
            {
                return BadRequest();
            }



            _context.Entry(personaje).State = EntityState.Modified;

            await _context.SaveChangesAsync();


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonajeExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }
            return Ok();
        }


        /// <DELETECHARACTER>
        /// 
        /// </Ingresar el id del personaje como value para que este sea borrado de la base de datos.>

        [HttpDelete("/Listado/Character/delete/{id}")]
        public async Task<ActionResult<Personaje>> DeleteCharacter(int id)
        {
            var personaje = await _context.Personajes.FindAsync(id);
            if (personaje == null)
            {
                return NotFound();
            }
            _context.Personajes.Remove(personaje);
            await _context.SaveChangesAsync();
            return personaje;

        }


        ////////////////Pelicula/////////

        /// <LISTADOMOVIE>
        /// https://localhost:7105/Listado/movies
        /// </Retorna Listado de peliculas>

        [HttpGet("/Listado/movies")]
        public async Task<ActionResult<IEnumerable<PeliculaOserieDTO>>> ListadoDePeliculas()
        {
            return await _context.PeliculasOseries
                .Select(x => PeliculaOserieToDTO(x))
                .ToListAsync();
        }



        /// <GETDETALLEMOVIE>
        /// Utilizar nombre luego  del endpoint  Eje : "https://localhost:7105/Detalle/Movie/Shrek"
        /// https://localhost:7105/Detalle/Movie/
        /// </Retorna un personaje con el correspondiente Titulo de la pelicula de la participa>

        [HttpGet("/Detalle/Movie/{MovieName}")]
        public ActionResult DetalleMovie(string MovieName)
        {

            int peliculaId = _context.PeliculasOseries.Where(x => x.Titulo.Equals(MovieName))
                                               .Select(x => x.MovieId).FirstOrDefault();


            var personajePelicula = _context.Personajes.Join(_context.PeliculasOseries, personaje => personaje.MovieId,
                                     pelicula => pelicula.MovieId, (personaje, pelicula) => new { pelicula, personaje })
                                    .Where(x => x.personaje.MovieId == peliculaId);


            return Ok(personajePelicula
                                       .Where(x => x.pelicula.MovieId.Equals(peliculaId))
                                       .Select(x => new {
                                           x.personaje.PeliculaOserie.Titulo,
                                           x.personaje.PeliculaOserie.MovieId,
                                           x.personaje.PeliculaOserie.Imagen,
                                           x.personaje.PeliculaOserie.FechaDeCreacion,
                                           x.personaje.PeliculaOserie.Calificacion,
                                           x.personaje.Nombre

                                       }));

        }


        /// <SEARCHMOVIE>
        /// Utilizar Query a partir de las siguientes URL
        /// https://localhost:7105/Busqueda/Movies?name="NOMBRE"
        /// https://localhost:7105/Busqueda/Movies?order="ASC"or"DESC"
        /// https://localhost:7105/Busqueda/Movies?genre="GenreId"
        /// </Retorna Query de busqueda de Movies>

        [HttpGet("/Busqueda/movies")]
        public async Task<ActionResult<List<PeliculaOserieDTO>>> SearchMovies([FromQuery] string? name, string? order, int? genre)
        {

            var peliculaQueryable = _context.PeliculasOseries.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                peliculaQueryable = peliculaQueryable.Where(x => x.Titulo.Contains(name));
            }

            if (!string.IsNullOrEmpty(order))
            {
                if (order == "ASC")
                {

                    peliculaQueryable = peliculaQueryable.OrderBy("FechaDeCreacion ascending");

                }
                if (order == "DESC")
                {

                    peliculaQueryable = peliculaQueryable.OrderBy("FechaDeCreacion descending");

                }
            }

            if (genre != null)
            {
                peliculaQueryable = peliculaQueryable.Where(x => x.GenreId.Equals(genre));

            }


             return Ok(peliculaQueryable
                                      
                                       .Select(x => new {
                                           x.Titulo,
                                           x.GenreId,

                                           }));
        }


        /// <POSTMOVIE>
        /// 
        /// </ADVERTENCIA!,MovieId es identidad ,es decir dejar en Valor 0 que actualizara automaticamente>

        [HttpPost("/Listado/Post/movie")]
        public async Task<ActionResult<PeliculaOserie>> PostMovie([FromBody] PeliculaDTOtoPost peliculaDTO)
        {
            PeliculaOserie peliculaoSerie = new()
            {
                MovieId = peliculaDTO.MovieId,
                Titulo = peliculaDTO.Titulo,
                Imagen = peliculaDTO.Imagen,
                FechaDeCreacion = peliculaDTO.FechaDeCreacion,
                Calificacion = peliculaDTO.Calificacion,
                PersonajesAsociados = peliculaDTO.PersonajesAsociados,
                GenreId = peliculaDTO.GenreId
            };
            _context.PeliculasOseries.Add(peliculaoSerie);
            await _context.SaveChangesAsync();
            return CreatedAtAction("ListadoDePeliculas", new { id = peliculaoSerie.MovieId }, peliculaoSerie);

        }
        
        /// <PUTMOVIE>
        /// 
        /// </ingresar id de la pelicula como Value, y tambien dentro del BODY>
        [HttpPut("/Listado/movie/{id}")]
        public async Task<ActionResult<PeliculaOserie>> MovieModification(int id,PeliculaDTOtoPost peliput)
        {

            PeliculaOserie peliculaoSerie = new()
            {
                MovieId = peliput.MovieId,
                Titulo = peliput.Titulo,
                Imagen = peliput.Imagen,
                FechaDeCreacion = peliput.FechaDeCreacion,
                Calificacion = peliput.Calificacion,
                PersonajesAsociados = peliput.PersonajesAsociados,
                GenreId = peliput.GenreId
            };

            if (id!=peliculaoSerie.MovieId)
            {
                return BadRequest();
            }

            _context.Entry(peliculaoSerie).State=EntityState.Modified;

            await _context.SaveChangesAsync();

            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!PeliculaExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
                
            }
            return Ok();
        }

        /// <DELETEMOVIE>
        /// 
        /// </Ingresar el id de la pelicula como value para que esta sea borrada de la base de datos.>
        
        [HttpDelete("/Listado/Movie/delete/{id}")]
        public async Task<ActionResult<PeliculaOserie>> DeleteMovie(int id)
        {
            var pelicula = await _context.PeliculasOseries.FindAsync(id);
            if(pelicula==null)
            {
                return NotFound();
            }
            _context.PeliculasOseries.Remove(pelicula);
            await _context.SaveChangesAsync();
            return pelicula;

        }


        private bool PeliculaExist(int id)
        {
            return _context.PeliculasOseries.Any(e => e.MovieId == id);
        }
        private bool PersonajeExist(int id)
        {
            return _context.Personajes.Any(e => e.CharacterId == id);
        }



        private static PersonajeDTO PersonajeToDTO(Personaje todoItem) =>
        new ()
        {
          CharacterId=todoItem.CharacterId,
          Nombre = todoItem.Nombre,
          Imagen = todoItem.Imagen,

        };
      

        private static PeliculaOserieDTO PeliculaOserieToDTO(PeliculaOserie peliculaOserie) =>
        new()
        {
           MovieId=peliculaOserie.MovieId,
           Titulo= peliculaOserie.Titulo,
           Imagen= peliculaOserie.Imagen,
           FechaDeCreacion= peliculaOserie.FechaDeCreacion
        };
       


    }

}

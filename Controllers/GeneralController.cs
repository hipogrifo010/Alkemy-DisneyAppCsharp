using Microsoft.AspNetCore.Mvc;
using ApiRestAlchemy.Database;
using ApiRestAlchemy.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NuGet.Protocol;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using ApiRestAlchemy.Database.ViewModel;
using System.Diagnostics;

namespace ApiRestAlchemy.Controllers
{
   

   [Route("api/[controller]")]
  //[Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]

    [ApiController]
    public class GeneralController : ControllerBase
    {
        public static IWebHostEnvironment _webHostEnvironment;

        private DatabaseContext _context;
        public GeneralController(DatabaseContext context,IWebHostEnvironment env)
        {
            _webHostEnvironment = env;
            _context = context;
        }


                
        /////////////Personaje///////////
  
        /// <LISTADOCHARACTERS>
        /// https://localhost:7105/Listado/characters
        /// </Retorna Listado de personajes>
        
        [HttpGet("/characters")]
        public  ActionResult ListadoPersonajes()
        {
            
            return Ok(_context.Personajes
                        .Select(x => new {
                            x.Nombre,
                            x.Imagen,
                            }));
        }



        /// <DETALLLECHARACTER>
        /// Utilizar nombre luego  del endpoint  Eje : "https://localhost:7105/DetalleCharacter/Woody"
        /// https://localhost:7105/DetalleCharacter/
        /// </Retorna un personaje con el correspondiente Titulo de la pelicula de la participa>

        [HttpGet("/characters/{CharacterName}")]
        public ActionResult DetalleCharacter(string CharacterName)
        {


            int thisCharacNameByMovieId = _context.Personajes.Where(x => x.Nombre.Equals(CharacterName))
                                               .Select(x => x.MovieId).FirstOrDefault();


            var personajeYpelicula = _context.Personajes.Join(_context.PeliculasOseries, personaje => personaje.MovieId,
                                     pelicula => pelicula.MovieId, (personaje, pelicula) => new { pelicula, personaje })
                                    .Where(x => x.personaje.MovieId == thisCharacNameByMovieId);


            return Ok(personajeYpelicula
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
        [HttpGet("/searchresult/characters")]
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

        [HttpPost("/new/characters")]
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

        /// <summary>
        /// POST Y GET IMAGENES
        /// </summary>
        /// 
        [HttpGet("/get/imagecharacter")]
        public IActionResult GetMovieImage(string name)
        {
            var personaje = _context.Personajes.FirstOrDefault(a => a.Nombre == name);

            byte[] b = System.IO.File.ReadAllBytes(_webHostEnvironment.ContentRootPath + "Img\\" + personaje.Imagen);   // You can use your own method over here.         
            return File(b, "image/jpg");

        }
        [HttpPost("/new/imagecharacter")]
        public async Task<ActionResult<PeliculaOserie>> PostMovieImage(string name, [Bind] IFormFile files)

        {

            var personaje = _context.Personajes.FirstOrDefault(a => a.Nombre == name);
            var id = personaje.MovieId;
            var filePath = _webHostEnvironment.ContentRootPath + "Img\\" + files.FileName;
            using (var stream = System.IO.File.Create(filePath))
            {
                files.CopyToAsync(stream);
            }
            personaje.Imagen = files.FileName;

            try
            {

                _context.Entry(personaje).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeliculaExist(id))
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
        /// <PUTCHARACTERS>
        /// 
        /// </ingresar id del Personaje como Value,y tambien dentro del BODY>
        [HttpPut("/edit/characters/{id}")]
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

        [HttpDelete("/delete/characters/{id}")]
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

        [HttpGet("/movies")]
        public ActionResult ListadoDePeliculas()
        {
            return Ok(_context.PeliculasOseries
             .Select(x => new {
                 Titulo = x.Titulo,
                 Imagen = x.Imagen,
                 FechaDeCreacion = x.FechaDeCreacion
             }));
 
        }

        /// <GETDETALLEMOVIE>
        /// Utilizar nombre luego  del endpoint  Eje : "https://localhost:7105/movies/Shrek"
        /// https://localhost:7105/movies/{}
        /// </Retorna un personaje con el correspondiente Titulo de la pelicula de la participa>

        [HttpGet("/movies/{MovieName}")]
        public ActionResult DetalleMovie(string MovieName)
        {
            
            int movieNameById = _context.PeliculasOseries.Where(x => x.Titulo.Equals(MovieName))
                                               .Select(x => x.MovieId).FirstOrDefault();


            var characterByMovieId = _context.Personajes.Join(_context.PeliculasOseries, personaje => personaje.MovieId,
                                     pelicula => pelicula.MovieId, (personaje, pelicula) => new { pelicula, personaje })
                                    .Where(x => x.personaje.MovieId == movieNameById);

         
            
            var moviebydetails = _context.PeliculasOseries.Where(x => x.Titulo.Equals(MovieName))
                .Select(x => new {
                            x.Titulo,
                            x.MovieId,
                            x.Imagen,
                            x.FechaDeCreacion,
                            x.Calificacion,
                            x.PersonajesAsociados,
                            x.GenreId
                        }).Distinct();

            var onlyCharacters = characterByMovieId.Where(x => x.personaje.PeliculaOserie.MovieId.Equals(movieNameById)).Select(x => new
            { x.personaje.Nombre});


            if (_context.Personajes.Any(x => x.MovieId == movieNameById))
            {
                var allcharactedlisted = _context.Personajes.Select(x => x.Nombre);
                return Ok(new { moviebydetails, allcharactedlisted });

            }
            else {
                string[] allcharactedlisted = new string[] { "Ninguno" };
                return Ok(new { moviebydetails, allcharactedlisted });
            }

        }


        /// <SEARCHMOVIE>
        /// Utilizar Query a partir de las siguientes URL
        /// https://localhost:7105/Busqueda/Movies?name="NOMBRE"
        /// https://localhost:7105/Busqueda/Movies?order="ASC"or"DESC"
        /// https://localhost:7105/Busqueda/Movies?genre="GenreId"
        /// </Retorna Query de busqueda de Movies>

        [HttpGet("/searchresult/movies")]
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
        /// 
        [HttpGet("/get/imagemovie")]
        public IActionResult GetImage(string name)
        {
            var pelicula = _context.PeliculasOseries.FirstOrDefault(a => a.Titulo == name);
   
            byte[] b = System.IO.File.ReadAllBytes(_webHostEnvironment.ContentRootPath + "Img\\" + pelicula.Imagen);   // You can use your own method over here.         
            return File(b,"image/jpg");

        }
        [HttpPost("/new/imagemovie")]
        public async Task<ActionResult<PeliculaOserie>> PostMovie(string name ,[Bind]IFormFile files)

        {
            
            var pelicula = _context.PeliculasOseries.FirstOrDefault(a => a.Titulo == name);
            var id = pelicula.MovieId;
            var filePath = _webHostEnvironment.ContentRootPath + "Img\\" + files.FileName;
            using (var stream = System.IO.File.Create(filePath))
            {
                files.CopyToAsync(stream);
            }
            pelicula.Imagen = files.FileName ;
           
            try
            {

                _context.Entry(pelicula).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeliculaExist(id))
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
        [HttpPost("/new/movies")]
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
            _context.SaveChanges();
            await _context.SaveChangesAsync();
            return CreatedAtAction("ListadoDePeliculas", new { id = peliculaoSerie.MovieId }, peliculaoSerie);


        }
       

        /// <PUTMOVIE>
        /// 
        /// </ingresar id de la pelicula como Value, y tambien dentro del BODY>
        [HttpPut("/edit/movies/{id}")]
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
        
        [HttpDelete("/delete/movies/{id}")]
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
      

  
       


    }

}

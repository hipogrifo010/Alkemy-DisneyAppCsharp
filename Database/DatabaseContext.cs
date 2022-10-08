using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApiRestAlchemy.Models;


namespace ApiRestAlchemy.Database
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
      
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        
        public DbSet<Personaje> Personajes { get; set; }
        public DbSet<PeliculaOserie> PeliculasOseries { get; set; }
        public DbSet<Genero> Generos { get; set; }


           //en caso de usar CodeFirst a Sql
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         modelBuilder.Entity<Personaje>().HasOne(p => p.PeliculaOserie).WithMany().HasForeignKey(x=>x.MovieId);

            ///////////Peliculas////////////


            modelBuilder.Entity<PeliculaOserie>().HasData(
            new PeliculaOserie { 
                MovieId = 1,
                Imagen = " ",
                Titulo = "Shrek",
                FechaDeCreacion = new DateTime(2001, 4, 22),
                Calificacion = 4,
                PersonajesAsociados="Shrek,Burro,Fiora",
                GenreId = 1,
                
            });
            modelBuilder.Entity<PeliculaOserie>().HasData(
            new PeliculaOserie { 
                MovieId = 2,
                Imagen = " ",
                Titulo = "Toy Story",
                FechaDeCreacion= new DateTime(1995, 11, 19),
                Calificacion = 4,
                PersonajesAsociados="Woody,Buzz Lightyear,Andy",
                GenreId = 2,


            });
            ///////////Personajes////////////
            modelBuilder.Entity<Personaje>().HasData(
            new Personaje { 
                CharacterId = 1,
                MovieId = 1,
                Nombre = "Shrek",
                Imagen = " ",
                Peso ="230kg",
                Edad = 30,
                Historia= "Un ogro llamado Shrek vive en su pantano, pero su preciada soledad se ve súbitamente interrumpida por la invasión de los ruidosos personajes de los cuentos de hadas"
            });

            modelBuilder.Entity<Personaje>().HasData(
            new Personaje { 
                CharacterId = 2,
                MovieId = 1,
                Nombre = "Burro",
                Imagen = " ",
                Peso ="200kg",
                Edad = 16,
                Historia= " es un asno parlante que se convierte en el compañero de aventuras de Shrek y se hace su mejor amigo, aunque al principio el ogro no le soportaba y lo repudia, cuando Lord Farquaad decide eliminar a las criaturas mágicas del reino y su dueña decide entregarlo."
            });

            modelBuilder.Entity<Personaje>().HasData(
            new Personaje
            {
                CharacterId = 3,
                MovieId = 2,
                Nombre = "Woody",
                Imagen = " ",
                Peso = "160g",
                Edad = 17,
                Historia = "Woody es un vaquero de juguete con una cuerda en la parte de atrás, que al tirar de ella dice frases tales como: Hay una serpiente en mi bota o Alguien ha envenenado el abrevadero. Woody es el juguete preferido de Andy,hasta que llega Buzz Lightyear"
            });

            modelBuilder.Entity<Personaje>().HasData(
            new Personaje
            {
                CharacterId = 4,
                MovieId = 2,
                Nombre = "Buzz Lightyear",
                Imagen = " ",
                Peso = "1kg",
                Edad = 16,
                Historia = " es un juguete con forma de guerrero espacial, el cual llega hasta las manos de Andy, un niño con una gran colección de juguetes. En casa de Andy conocerá al resto de juguetes como son Woody, el Sr. Patata o Rex, entre otros."
            });
            ///////////Generos////////////
            modelBuilder.Entity<Genero>().HasData(
            new Genero
            {
                GenreId = 1,
                Nombre = "Aventura/Fantasia",
                Image = " ",
                
            });

            modelBuilder.Entity<Genero>().HasData(
            new Genero
            {
                GenreId = 2,
                Nombre = "Infantil/Fantasia",
                Image = " ",
                
            });

            ///////////////////////
            modelBuilder.Entity<Personaje>().ToTable("Personaje");
            modelBuilder.Entity<PeliculaOserie>().ToTable("PeliculaOserie");
            modelBuilder.Entity<Genero>().ToTable("Genero");
            base.OnModelCreating(modelBuilder);

        }


           //en caso de usar CodeFirst a Sql
        public DbSet<ApiRestAlchemy.Models.PeliculaOserieDTO>? PeliculaOserieDTO { get; set; }
        
        
    }
}

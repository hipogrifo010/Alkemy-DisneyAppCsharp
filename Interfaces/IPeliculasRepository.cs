using ApiRestAlchemy.Models;
using System.Diagnostics.Metrics;

namespace ApiRestAlchemy.Interfaces
{
    public interface IPeliculasRepository
    {
        public IEnumerable<PeliculaOserie> GetallMovies();
      

        Task Save();
    }
}

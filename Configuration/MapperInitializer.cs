using ApiRestAlchemy.Models;
using AutoMapper;

namespace ApiRestAlchemy.Configuration
{
    public class MapperInitializer:Profile
    {
        public MapperInitializer()
        {
            CreateMap<PeliculaOserie, PeliculaOserieDTO>().ReverseMap();
        }
    }
}

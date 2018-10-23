using AutoMapper;
using Models = DataLoaderWithEFCore.Data.Models;

namespace DataLoaderWithEFCore.GraphApi
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Models.Actor, Schema.Actor>();
            CreateMap<Models.Country, Schema.Country>();
            CreateMap<Models.Movie, Schema.Movie>();
        }
    }
}

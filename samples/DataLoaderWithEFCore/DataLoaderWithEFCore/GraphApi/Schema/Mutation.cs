using System.Threading.Tasks;
using AutoMapper;
using DataLoaderWithEFCore.Data.Repositories;
using GraphQL.Conventions;

namespace DataLoaderWithEFCore.GraphApi.Schema
{
    public sealed class Mutation
    {
        private readonly IMapper _mapper;

        public Mutation(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<Movie> UpdateMovieTitle([Inject] IMovieRepository movieRepository, UpdateMovieTitleParams @params)
            => _mapper.Map<Movie>(await movieRepository.UpdateMovieTitle(@params.Id, @params.NewTitle));
    }
}

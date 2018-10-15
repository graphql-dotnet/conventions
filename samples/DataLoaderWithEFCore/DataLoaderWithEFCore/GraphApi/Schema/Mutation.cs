using System.Threading.Tasks;
using AutoMapper;
using DataLoaderWithEFCore.Data.Repositories;
using GraphQL.Conventions;

namespace DataLoaderWithEFCore.GraphApi.Schema
{
    public sealed class Mutation
    {
        public async Task<Movie> UpdateMovieTitle([Inject] IMovieRepository movieRepository, UpdateMovieTitleParams @params)
            => Mapper.Map<Movie>(await movieRepository.UpdateMovieTitle(@params.Id, @params.NewTitle));
    }
}

using System;
using System.Threading.Tasks;
using AutoMapper;
using DataLoaderWithEFCore.Data.Repositories;
using GraphQL.Conventions;

namespace DataLoaderWithEFCore.GraphApi.Schema
{
    public sealed class Query
    {
        public async Task<Movie> Movie([Inject] IMovieRepository repository, Guid id)
            => Mapper.Map<Movie>(await repository.FindMovie(id).ConfigureAwait(false));

        public Task<Movie[]> Movies([Inject] IMovieRepository repository)
        {
            return Task.Run(async () => Mapper.Map<Movie[]>(await repository.GetMovies().ConfigureAwait(false)));
        }
    }
}

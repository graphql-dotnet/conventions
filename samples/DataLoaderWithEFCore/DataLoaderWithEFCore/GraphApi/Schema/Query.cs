using AutoMapper;
using DataLoaderWithEFCore.Data.Repositories;
using GraphQL.Conventions;

namespace DataLoaderWithEFCore.GraphApi.Schema;

public sealed class Query
{
    private readonly IMapper _mapper;

    public Query(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<Movie> Movie([Inject] IMovieRepository repository, Guid id)
    {
        return _mapper.Map<Movie>(await repository.FindMovie(id));
    }

    public async Task<Movie[]> Movies([Inject] IMovieRepository repository)
        => _mapper.Map<Movie[]>(await repository.GetMovies());
}

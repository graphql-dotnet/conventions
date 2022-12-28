using DataLoaderWithEFCore.Data.Models;
using DataLoaderWithEFCore.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataLoaderWithEFCore.Tests;

public class TestRepository : IActorRepository, ICountryRepository, IMovieRepository
{
    private readonly List<Movie> _movies = new();
    private readonly List<Country> _countries = new();
    private readonly List<Actor> _actors = new();

    public TestRepository()
    {
        var movie1 = new Movie { Id = Guid.Parse("b08dcbc7-7934-44f8-b059-d80c5b9d6a24"), Title = "Johnny English Strikes Again", Genre = "Action/Adventure", ReleaseDateUtc = DateTime.Parse("10/12/2018 00:00:00Z") };
        var movie2 = new Movie { Id = Guid.Parse("071bb926-b4bc-46c1-8df4-f6693b3cdcf3"), Title = "A Star Is Born", Genre = "Drama/Romance", ReleaseDateUtc = DateTime.Parse("10/04/2018 00:00:00Z") };

        _actors.AddRange(new Actor[] {
            new Actor { Id = Guid.Parse("7848ff37-4ded-4de2-8f43-33ceb14be749"), CountryCode = "UK", Name = "Rowan Atkinson", MovieId = movie1.Id },
            new Actor { Id = Guid.Parse("66c94695-6ae2-491c-a03e-38dcf5dfaf36"), CountryCode = "FR", Name = "Olga Kurylenko", MovieId = movie1.Id },
            new Actor { Id = Guid.Parse("2dffa016-0c02-4e07-8795-ae87a561272e"), CountryCode = "US", Name = "Jake Lacy", MovieId = movie1.Id },
            new Actor { Id = Guid.Parse("32fc0cb7-0b3f-44ec-a4e6-7ca8541869e1"), CountryCode = "US", Name = "Bradley Cooper", MovieId = movie2.Id },
            new Actor { Id = Guid.Parse("ac403490-6faa-4807-a1ec-1dc67c5754c9"), CountryCode = "US", Name = "Lady Gaga", MovieId = movie2.Id },
            new Actor { Id = Guid.Parse("cb915c13-9ac9-495e-866b-4cc320693396"), CountryCode = "US", Name = "Sam Elliott", MovieId = movie2.Id },
            new Actor { Id = Guid.Parse("d1833be8-f805-41ee-b0c2-7073682afcde"), CountryCode = "US", Name = "Andrew Dice Clay", MovieId = movie2.Id },
            new Actor { Id = Guid.Parse("897f3cb9-d7f8-4b6e-b6b6-bf19613226fa"), CountryCode = "US", Name = "Dave Chappelle", MovieId = movie2.Id },
            new Actor { Id = Guid.Parse("1ca75994-a496-4225-80f5-058c21357cd3"), CountryCode = "US", Name = "Rebecca Field", MovieId = movie2.Id },
            new Actor { Id = Guid.Parse("31adb28f-064f-417c-87a9-75ea3281a6db"), CountryCode = "US", Name = "Michael Harney", MovieId = movie2.Id },
            new Actor { Id = Guid.Parse("3126ece0-5987-47e6-9ee3-91bc4f0abada"), CountryCode = "US", Name = "Rafi Gavron", MovieId = movie2.Id },
            new Actor { Id = Guid.Parse("988587cc-f41c-4651-9006-5ef96602b468"), CountryCode = "US", Name = "Willam Belli", MovieId = movie2.Id },
            new Actor { Id = Guid.Parse("8afa0dfc-3707-4494-a2fc-4e0b309d1959"), CountryCode = "US", Name = "Halsey", MovieId = movie2.Id },
        });

        _countries.AddRange(new Country[] {
            new Country { Code = "UK", Name = "United Kingdom" },
            new Country { Code = "FR", Name = "France" },
            new Country { Code = "US", Name = "United States" },
        });

        _movies.AddRange(new Movie[]
        {
            movie1,
            movie2,
        });
    }

    public Task<Movie> FindMovie(Guid id)
        => Task.FromResult(_movies.Single(x => x.Id == id));

    public Task<ILookup<Guid, Actor>> GetActorsPerMovie(IEnumerable<Guid> movieIds)
        => Task.FromResult(_actors.Where(x => movieIds.Contains(x.MovieId)).ToLookup(x => x.MovieId));

    public Task<IDictionary<string, Country>> GetCountries(IEnumerable<string> countryCodes)
        => Task.FromResult<IDictionary<string, Country>>(_countries.Where(x => countryCodes.Contains(x.Code)).ToDictionary(x => x.Code));

    public Task<Movie[]> GetMovies()
        => Task.FromResult(_movies.ToArray());

    public Task<Movie> UpdateMovieTitle(Guid id, string newTitle)
    {
        var movie = _movies.Single(x => x.Id == id);
        movie.Title = newTitle;
        return Task.FromResult(movie);
    }
}

using System;
using System.Threading.Tasks;
using DataLoaderWithEFCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLoaderWithEFCore.Data.Repositories
{
    public interface IMovieRepository
    {
        Task<Movie> FindMovie(Guid id);

        Task<Movie[]> GetMovies();
    }

    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDbContext _context;

        public MovieRepository(MovieDbContext context)
        {
            _context = context;
        }

        public Task<Movie> FindMovie(Guid id)
            => _context.Movies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public Task<Movie[]> GetMovies()
            => _context.Movies.AsNoTracking().ToArrayAsync();
    }
}

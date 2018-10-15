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

        public async Task<Movie> FindMovie(Guid id)
            => await _context.Movies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Movie[]> GetMovies()
            => await _context.Movies.AsNoTracking().ToArrayAsync();
    }
}

using System;
using System.Linq;
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
            => Task.FromResult(_context.Movies.AsNoTracking().FirstOrDefault(x => x.Id == id));

        public Task<Movie[]> GetMovies()
            => Task.FromResult(_context.Movies.AsNoTracking().ToArray());
    }
}

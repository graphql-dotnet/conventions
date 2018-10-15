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

        Task<Movie> UpdateMovieTitle(Guid id, string newTitle);
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

        public async Task<Movie> UpdateMovieTitle(Guid id, string newTitle)
        {
            var movie = await FindMovie(id);
            if (movie == null)
                throw new InvalidOperationException($"Movie with id {id} not found");

            movie.Title = newTitle;
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
            return movie;
        }
    }
}

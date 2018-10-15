using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLoaderWithEFCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLoaderWithEFCore.Data.Repositories
{
    public interface IActorRepository
    {
        Task<ILookup<Guid, Actor>> GetActorsPerMovie(IEnumerable<Guid> movieIds);
    }

    public class ActorRepository : IActorRepository
    {
        private readonly MovieDbContext _context;

        public ActorRepository(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<ILookup<Guid, Actor>> GetActorsPerMovie(IEnumerable<Guid> movieIds)
            => (await _context.Actors
                .AsNoTracking()
                .Where(x => movieIds.Contains(x.MovieId))
                .ToArrayAsync())
                .ToLookup(x => x.MovieId);
    }
}

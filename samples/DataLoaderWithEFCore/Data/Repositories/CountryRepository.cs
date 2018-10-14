using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLoaderWithEFCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLoaderWithEFCore.Data.Repositories
{
    public interface ICountryRepository
    {
        Task<IDictionary<string, Country>> GetCountries(IEnumerable<string> countryCodes);
    }

    public class CountryRepository : ICountryRepository
    {
        private readonly MovieDbContext _context;

        public CountryRepository(MovieDbContext context)
        {
            _context = context;
        }

        public Task<IDictionary<string, Country>> GetCountries(IEnumerable<string> countryCodes)
            => Task.FromResult((IDictionary<string, Country>)_context.Countries
                .AsNoTracking()
                .Where(x => countryCodes.Contains(x.Code))
                .ToDictionary(x => x.Code));
    }
}

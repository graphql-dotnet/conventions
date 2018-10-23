using System;
using System.Threading.Tasks;
using AutoMapper;
using DataLoaderWithEFCore.Data.Repositories;
using GraphQL.Conventions;
using GraphQL.DataLoader;
using Models = DataLoaderWithEFCore.Data.Models;

namespace DataLoaderWithEFCore.GraphApi.Schema
{
    public sealed class Actor
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string CountryCode { get; set; }

        public Guid MovieId { get; set; }

        public async Task<Country> Country([Inject] ICountryRepository repository, [Inject] DataLoaderContext dataLoaderContext)
        {
            var loader = dataLoaderContext.GetOrAddBatchLoader<string, Models.Country>("Actor_Country", repository.GetCountries);
            return Mapper.Map<Country>(await loader.LoadAsync(CountryCode));
        }
    }
}

using System;
using System.Threading.Tasks;
using AutoMapper;
using DataLoaderWithEFCore.Data.Repositories;
using GraphQL.Conventions;
using GraphQL.DataLoader;
using Models = DataLoaderWithEFCore.Data.Models;

namespace DataLoaderWithEFCore.GraphApi.Schema
{
    public sealed class Movie
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Genre { get; set; }

        public DateTime ReleaseDateUtc { get; set; }

        public async Task<Actor[]> Actors([Inject] IActorRepository repository, [Inject] DataLoaderContext dataLoaderContext)
        {
            var loader = dataLoaderContext.GetOrAddCollectionBatchLoader<Guid, Models.Actor>("Movie_Actors", repository.GetActorsPerMovie);
            return Mapper.Map<Actor[]>(await loader.LoadAsync(Id));
        }
    }
}

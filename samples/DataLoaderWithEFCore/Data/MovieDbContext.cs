using System;
using DataLoaderWithEFCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLoaderWithEFCore.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Actor> Actors { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var movie1 = new Movie { Id = Guid.NewGuid(), Title = "Johnny English Strikes Again", Genre = "Action/Adventure", ReleaseDateUtc = DateTime.Parse("10/12/2018 00:00:00Z") };
            var movie2 = new Movie { Id = Guid.NewGuid(), Title = "A Star Is Born", Genre = "Drama/Romance", ReleaseDateUtc = DateTime.Parse("10/04/2018 00:00:00Z") };

            modelBuilder.Entity<Actor>().HasData(
                new Actor { Id = Guid.NewGuid(), CountryCode = "UK", Name = "Rowan Atkinson",   MovieId = movie1.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "FR", Name = "Olga Kurylenko",   MovieId = movie1.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Jake Lacy",        MovieId = movie1.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Bradley Cooper",   MovieId = movie2.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Lady Gaga",        MovieId = movie2.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Sam Elliott",      MovieId = movie2.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Andrew Dice Clay", MovieId = movie2.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Dave Chappelle",   MovieId = movie2.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Rebecca Field",    MovieId = movie2.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Michael Harney",   MovieId = movie2.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Rafi Gavron",      MovieId = movie2.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Willam Belli",     MovieId = movie2.Id },
                new Actor { Id = Guid.NewGuid(), CountryCode = "US", Name = "Halsey",           MovieId = movie2.Id });

            modelBuilder.Entity<Country>().HasData(
                new Country { Code = "UK", Name = "United Kingdom" },
                new Country { Code = "FR", Name = "France" },
                new Country { Code = "US", Name = "United States" });

            modelBuilder.Entity<Movie>().HasData(movie1, movie2);
        }
    }
}

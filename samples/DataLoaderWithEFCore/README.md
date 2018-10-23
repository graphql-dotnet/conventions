# DataLoader with EF Core example

## Creating the test database

In the Package Manager Console, type:

    Update-Database

## Test GraphQL endpoint

To test the endpoint, use a GraphiQL client or similar.

We have used the [GraphiQL Electron client](https://electronjs.org/apps/graphiql) in our tests.

Start the project in Visual Studio and connect the GraphiQL client with the endpoint:

    https://localhost:44329/api/graph

### Query all data

```
query AllMovies {
  movies {
    id
    title
    genre
    releaseDateUtc
    
    actors {
      id
      name
      
      country {
        code
        name
      }
    }
  }
}
```

### Update movie title

```
mutation UpdateTitle {
  updateMovieTitle(params: {
    id: "95e03cd2-1b4d-4300-8d7c-56bdf8aae646"
    newTitle: "A Baby Is Born"
  }) {
    id
    title
  }
}
```

using System.Net;
using System.Text.Json;
using DataLoaderWithEFCore.Data.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DataLoaderWithEFCore.Tests;

public class EndToEndTests
{
    [Fact]
    public async Task Search()
    {
        var query = """
            {
              movies {
                id
                title
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
            """;

        var expected = """
            {
              "data":{
                "movies":[
                  {
                    "id":"b08dcbc7-7934-44f8-b059-d80c5b9d6a24",
                    "title":"Johnny English Strikes Again",
                    "actors":[
                      {
                        "id":"7848ff37-4ded-4de2-8f43-33ceb14be749",
                        "name":"Rowan Atkinson",
                        "country":{
                          "code":"UK",
                          "name":"United Kingdom"
                        }
                      },
                      {
                        "id":"66c94695-6ae2-491c-a03e-38dcf5dfaf36",
                        "name":"Olga Kurylenko",
                        "country":{
                          "code":"FR",
                          "name":"France"
                        }
                      },
                      {
                        "id":"2dffa016-0c02-4e07-8795-ae87a561272e",
                        "name":"Jake Lacy",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      }
                    ]
                  },
                  {
                    "id":"071bb926-b4bc-46c1-8df4-f6693b3cdcf3",
                    "title":"A Star Is Born",
                    "actors":[
                      {
                        "id":"32fc0cb7-0b3f-44ec-a4e6-7ca8541869e1",
                        "name":"Bradley Cooper",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      },
                      {
                        "id":"ac403490-6faa-4807-a1ec-1dc67c5754c9",
                        "name":"Lady Gaga",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      },
                      {
                        "id":"cb915c13-9ac9-495e-866b-4cc320693396",
                        "name":"Sam Elliott",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      },
                      {
                        "id":"d1833be8-f805-41ee-b0c2-7073682afcde",
                        "name":"Andrew Dice Clay",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      },
                      {
                        "id":"897f3cb9-d7f8-4b6e-b6b6-bf19613226fa",
                        "name":"Dave Chappelle",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      },
                      {
                        "id":"1ca75994-a496-4225-80f5-058c21357cd3",
                        "name":"Rebecca Field",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      },
                      {
                        "id":"31adb28f-064f-417c-87a9-75ea3281a6db",
                        "name":"Michael Harney",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      },
                      {
                        "id":"3126ece0-5987-47e6-9ee3-91bc4f0abada",
                        "name":"Rafi Gavron",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      },
                      {
                        "id":"988587cc-f41c-4651-9006-5ef96602b468",
                        "name":"Willam Belli",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      },
                      {
                        "id":"8afa0dfc-3707-4494-a2fc-4e0b309d1959",
                        "name":"Halsey",
                        "country":{
                          "code":"US",
                          "name":"United States"
                        }
                      }
                    ]
                  }
                ]
              }
            }
            """;

        var testRepository = new TestRepository();
        using var webApp = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(config =>
            {
                config.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IActorRepository>(testRepository);
                    services.AddSingleton<ICountryRepository>(testRepository);
                    services.AddSingleton<IMovieRepository>(testRepository);
                });
            });
        using var server = webApp.Server;
        await VerifyGraphQLPostAsync(server, "/api/graph", query, expected).ConfigureAwait(false);
    }

    private static async Task VerifyGraphQLPostAsync(
        TestServer server,
        string url,
        string query,
        string expected,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        using var client = server.CreateClient();
        var body = JsonSerializer.Serialize(new { query });
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = content;
        using var response = await client.SendAsync(request).ConfigureAwait(false);
        Assert.Equal(statusCode, response.StatusCode);
        var ret = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var jsonExpected = JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(expected));
        var jsonActual = JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(ret));
        Assert.Equal(jsonExpected, jsonActual);
    }
}

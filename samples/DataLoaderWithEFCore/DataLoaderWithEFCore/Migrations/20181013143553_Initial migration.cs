using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLoaderWithEFCore.Migrations
{
    public partial class Initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    CountryCode = table.Column<string>(maxLength: 2, nullable: false),
                    MovieId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 2, nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 500, nullable: false),
                    Genre = table.Column<string>(maxLength: 50, nullable: false),
                    ReleaseDateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "Id", "CountryCode", "MovieId", "Name" },
                values: new object[,]
                {
                    { new Guid("5ff76e2f-699f-41b1-8521-e633afac4141"), "UK", new Guid("a9d69485-4980-467c-8afe-f385935d1b86"), "Rowan Atkinson" },
                    { new Guid("260d7ea3-0f21-49bf-8aa5-8ccf8bd62d22"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Halsey" },
                    { new Guid("4e79963d-00ad-4019-9d61-730505997022"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Willam Belli" },
                    { new Guid("e0d6cadf-ce5d-410c-8a19-5e20944051ce"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Rafi Gavron" },
                    { new Guid("67df57b2-a8d2-41e1-8c46-fe0e68514e9b"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Michael Harney" },
                    { new Guid("1baa4cc1-e148-4fa0-8e68-ed0089a0bdce"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Dave Chappelle" },
                    { new Guid("d84898af-ce94-4dd4-8c8e-4ee525a33117"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Rebecca Field" },
                    { new Guid("65d3c8d6-cef8-4647-acb0-c8d5c9cb5bbc"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Sam Elliott" },
                    { new Guid("8eb4ac98-6cca-4bfb-a067-bf9fd863fcc5"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Lady Gaga" },
                    { new Guid("a39df656-711b-4386-abbc-8deaf4f24d09"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Bradley Cooper" },
                    { new Guid("fe22e6da-c921-40b7-8b1b-782a03260e6d"), "US", new Guid("a9d69485-4980-467c-8afe-f385935d1b86"), "Jake Lacy" },
                    { new Guid("11423353-4745-44e9-91ce-65cb4bc0e8b7"), "FR", new Guid("a9d69485-4980-467c-8afe-f385935d1b86"), "Olga Kurylenko" },
                    { new Guid("fc30f61b-b013-4590-999e-4f0a93704015"), "US", new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Andrew Dice Clay" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { "UK", "United Kingdom" },
                    { "FR", "France" },
                    { "US", "United States" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Genre", "ReleaseDateUtc", "Title" },
                values: new object[,]
                {
                    { new Guid("a9d69485-4980-467c-8afe-f385935d1b86"), "Action/Adventure", new DateTime(2018, 10, 12, 2, 0, 0, 0, DateTimeKind.Local), "Johnny English Strikes Again" },
                    { new Guid("95e03cd2-1b4d-4300-8d7c-56bdf8aae646"), "Drama/Romance", new DateTime(2018, 10, 4, 2, 0, 0, 0, DateTimeKind.Local), "A Star Is Born" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}

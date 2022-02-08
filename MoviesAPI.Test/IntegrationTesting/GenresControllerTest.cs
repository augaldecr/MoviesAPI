using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Test.IntegrationTesting
{
    [TestClass]
    public class GenresControllerTest : TestBase
    {
        private static readonly string url = "api/genres";

        [TestMethod]
        public async Task GetGenresEmptyDB()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var genres = JsonConvert
                .DeserializeObject<List<GenreDTO>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(0, genres?.Count);
        }

        [TestMethod]
        public async Task GetGenres()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);

            var context = BuildContext(dbName);
            context.Genres.Add(new Genre { Name = "Genre 1" });
            context.Genres.Add(new Genre { Name = "Genre 2" });
            await context.SaveChangesAsync();

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var genres = JsonConvert
                .DeserializeObject<List<GenreDTO>>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(2, genres?.Count);
        }

        [TestMethod]
        public async Task DeleteGenre()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);

            var context = BuildContext(dbName);
            context.Genres.Add(new Genre { Name = "Genre 1" });
            await context.SaveChangesAsync();

            var client = factory.CreateClient();
            var response = await client.DeleteAsync($"{url}/1");
            response.EnsureSuccessStatusCode();

            var context2 = BuildContext(dbName);
            var exist = await context2.Genres.AnyAsync();
            Assert.IsFalse(exist);
        }

        [TestMethod]
        public async Task DeleteGenreReturn401()
        {
            var nameDB = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(nameDB, ignoreSecurity: false);

            var client = factory.CreateClient();
            var response = await client.DeleteAsync($"{url}/1");
            Assert.AreEqual("Unauthorized", response.ReasonPhrase);
        }
    }
}

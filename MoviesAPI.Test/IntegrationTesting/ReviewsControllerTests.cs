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
    public class ReviewsControllerTests : TestBase
    {
        private static readonly string url = "/api/movies/1/reviews";

        [TestMethod]
        public async Task GetsReviewsMovieNotFound()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);
            Assert.AreEqual(404, (int)response.StatusCode);
        }

        [TestMethod]
        public async Task GetReviewsEmptyList()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = BuildWebApplicationFactory(dbName);
            var context = BuildContext(dbName);
            context.Movies.Add(new Movie { Title = "Movie 1" });
            await context.SaveChangesAsync();

            var client = factory.CreateClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var reviews = JsonConvert.DeserializeObject<List<ReviewDTO>>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(0, reviews?.Count);
        }
    }
}

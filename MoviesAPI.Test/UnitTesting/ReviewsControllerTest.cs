using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Controllers;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Test.UnitTesting
{
    [TestClass]
    public class ReviewsControllerTest : TestBase
    {
        [TestMethod]
        public async Task UserCantCreateMoreThanOneReviewAsociatedWithTheSameMovie()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            CreateMovies(dbName);

            var movieId = context.Movies.Select(x => x.Id).First();
            var review1 = new Review()
            {
                MovieId = movieId,
                UserId = defaultUserId,
                Punctuation = 5
            };

            context.Add(review1);
            await context.SaveChangesAsync();

            var context2 = BuildContext(dbName);

            var controller = new ReviewsController(context2);
            controller.ControllerContext = BuildControllerContext();

            var reviewCreateDTO = new ReviewCreateDTO { Punctuation = 5 };
            var response = await controller.PostReview(movieId, reviewCreateDTO);

            var value = response as IStatusCodeActionResult;
            Assert.AreEqual(400, value?.StatusCode.Value);
        }

        [TestMethod]
        public async Task CreateReview()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            CreateMovies(dbName);

            var movieId = context.Movies.Select(x => x.Id).First();
            var context2 = BuildContext(dbName);

            var controller = new ReviewsController(context2);
            controller.ControllerContext = BuildControllerContext();

            var reviewCreateDTO = new ReviewCreateDTO() { Punctuation = 5 };
            var response = await controller.PostReview(movieId, reviewCreateDTO);

            var value = response as NoContentResult;
            Assert.IsNotNull(value);

            var context3 = BuildContext(dbName);
            var reviewDB = context3.Reviews.First();
            Assert.AreEqual(defaultUserId, reviewDB.UserId);
        }

        private void CreateMovies(string dbName)
        {
            var context = BuildContext(dbName);

            context.Movies.Add(new Movie() { Title = "Movie 1" });

            context.SaveChanges();
        }
    }
}

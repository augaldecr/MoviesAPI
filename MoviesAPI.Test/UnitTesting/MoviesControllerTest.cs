using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Test.UnitTesting
{
    [TestClass]
    public class MoviesControllerTest : TestBase
    {
        private string CreateTestData()
        {
            var databaseName = Guid.NewGuid().ToString();
            var context = BuildContext(databaseName);
            var genre = new Genre { Name = "genre 1" };

            var movies = new List<Movie>()
            {
                new Movie { Title = "Movie 1", ReleaseDate = new DateTime(2010, 1,1), OnBillboard = false},
                new Movie { Title = "Not released", ReleaseDate = DateTime.Today.AddDays(1), OnBillboard = false},
                new Movie { Title = "Movie on Cinemas", ReleaseDate = DateTime.Today.AddDays(-1), OnBillboard = true}
            };

            context.Add(genre);
            context.SaveChanges();

            var movieWithGenre = new Movie
            {
                Title = "Movie with genre",
                ReleaseDate = new DateTime(2010, 1, 1),
                OnBillboard = false,
                Genres = new Genre[] { genre }
            };

            movies.Add(movieWithGenre);
            context.AddRange(movies);
            context.SaveChanges();

            return databaseName;
        }

        [TestMethod]
        public async Task FilterByTitle()
        {
            var dbName = CreateTestData();
            var context = BuildContext(dbName);

            var controller = new MoviesController(context, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var movieTitle = "Movie 1";

            var filterDTO = new FilterMoviesDTO()
            {
                Title = movieTitle,
                QuantityPerPage = 10
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies.Length);
            Assert.AreEqual(movieTitle, movies?[0].Title);
        }

        [TestMethod]
        public async Task FilterByMovieTheater()
        {
            var dbName = CreateTestData();
            var context = BuildContext(dbName);

            var controller = new MoviesController(context, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                OnBillboard = true
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies?.Length);
            Assert.AreEqual("Movie on Cinemas", movies?[0].Title);
        }

        [TestMethod]
        public async Task FilterNextReleases()
        {
            var dbName = CreateTestData();
            var context = BuildContext(dbName);

            var controller = new MoviesController(context, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FilterMoviesDTO()
            {
                FutureReleases = true
            };

            var response = await controller.Filter(filtroDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies?.Length);
            Assert.AreEqual("Not released", movies?[0].Title);
        }

        [TestMethod]
        public async Task FilterByGenre()
        {
            var dbName = CreateTestData();
            var context = BuildContext(dbName);

            var controller = new MoviesController(context, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var genreId = context.Genres.Select(x => x.Id).First();

            var filterDTO = new FilterMoviesDTO()
            {
                GenreId = genreId
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;
            Assert.AreEqual(1, movies?.Length);
            Assert.AreEqual("Movie with genre", movies?[0].Title);
        }

        [TestMethod]
        public async Task FilterAscOrderTitle()
        {
            var dbName = CreateTestData();
            var context = BuildContext(dbName);

            var controller = new MoviesController(context, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                OrderField = "title",
                AscOrder = true
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;

            var context2 = BuildContext(dbName);
            var moviesDB = context2.Movies.OrderBy(x => x.Title).ToList();

            Assert.AreEqual(moviesDB.Count, movies?.Length);

            for (int i = 0; i < moviesDB.Count; i++)
            {
                var movieFromController = movies?[i];
                var movieDB = moviesDB[i];

                Assert.AreEqual(movieDB.Id, movieFromController?.Id);
            }
        }

        [TestMethod]
        public async Task FilterDescOrderTitle()
        {
            var dbName = CreateTestData();
            var context = BuildContext(dbName);

            var controller = new MoviesController(context, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                OrderField = "title",
                AscOrder = false
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;

            var context2 = BuildContext(dbName);
            var moviesDB = context2.Movies.OrderByDescending(x => x.Title).ToList();

            Assert.AreEqual(moviesDB.Count, movies?.Length);

            for (int i = 0; i < moviesDB.Count; i++)
            {
                var movieFromController = movies?[i];
                var movieDB = moviesDB[i];

                Assert.AreEqual(movieDB.Id, movieFromController?.Id);
            }
        }

        [TestMethod]
        public async Task FilterByIncorrectFieldReturnMovies()
        {
            var dbName = CreateTestData();
            var context = BuildContext(dbName);

            var mock = new Mock<ILogger<MoviesController>>();

            var controller = new MoviesController(context, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filterDTO = new FilterMoviesDTO()
            {
                OrderField = "abc",
                AscOrder = true
            };

            var response = await controller.Filter(filterDTO);
            var movies = response.Value;

            var context2 = BuildContext(dbName);
            var moviesDB = context2.Movies.ToList();
            Assert.AreEqual(moviesDB.Count, movies?.Length);
            Assert.AreEqual(1, mock.Invocations.Count);
        }
    }
}

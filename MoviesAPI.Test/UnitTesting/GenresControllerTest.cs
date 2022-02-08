using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class GenresControllerTest : TestBase
    {
        [TestMethod]
        public async Task GetAllTheGenres()
        {
            //Prep
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);

            await context.Genres.AddAsync(new Genre { Name = "Genre 1" });
            await context.Genres.AddAsync(new Genre { Name = "Genre 2" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);

            // Test
            var controller = new GenresController(context2);
            var response = await controller.GetGenres();

            //Verify
            var genres = response.Value;
            Assert.AreEqual(2, genres?.Count());
        }

        [TestMethod]
        public async Task GetNullGenreById()
        {
            //Prep

            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);

            // Test
            var controller = new GenresController(context);
            var response = await controller.GetGenre(1);

            //Verify
            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(404, result?.StatusCode);
        }

        [TestMethod]
        public async Task GetGenreById()
        {
            //Prep
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);

            await context.Genres.AddAsync(new Genre { Name = "Genre 1" });
            await context.Genres.AddAsync(new Genre { Name = "Genre 2" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);

            // Test
            var controller = new GenresController(context2);
            int id = 1;
            var response = await controller.GetGenre(id);

            //Verify
            var result = response.Value;
            Assert.AreEqual(id, result?.Id);
        }

        [TestMethod]
        public async Task CreateGenre()
        {
            //Prep
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);

            var newGenre = new GenreCreateDTO { Name = "New Genre" };

            // Test
            var controller = new GenresController(context);
            var response = await controller.PostGenre(newGenre);

            //Verify
            var result = response as CreatedAtActionResult;
            Assert.IsNotNull(result);

            var context2 = BuildContext(nameDB);
            var count = await context2.Genres.CountAsync();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task UpdateGenre()
        {
            //Prep
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);

            await context.Genres.AddAsync(new GenreCreateDTO { Name = "Genre 1" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            var newGenre = new GenreCreateDTO { Name = "New Genre" };

            // Test
            var controller = new GenresController(context);
            int id = 1;
            var response = await controller.PutGenre(id, newGenre);

            //Verify
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result?.StatusCode);

            var context3 = BuildContext(nameDB);
            var exist = await context3.Genres.AnyAsync(g => g.Name.Equals("New Genre"));
            Assert.IsTrue(exist);
        }

        [TestMethod]
        public async Task DeleteNullGenre()
        {
            //Prep
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);

            // Test
            var controller = new GenresController(context);
            int id = 1;
            var response = await controller.DeleteGenre(id);

            //Verify
            var result = response as StatusCodeResult;
            Assert.AreEqual(404, result?.StatusCode);
        }

        [TestMethod]
        public async Task DeleteGenre()
        {
            //Prep
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);

            await context.Genres.AddAsync(new GenreCreateDTO { Name = "Genre 1" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);
            var controller = new GenresController(context2);

            // Test
            int id = 1;
            var response = await controller.DeleteGenre(id);

            //Verify
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result?.StatusCode);

            var context3 = BuildContext(nameDB);
            var exist = await context3.Genres.AnyAsync();
            Assert.IsFalse(exist);
        }
    }
}

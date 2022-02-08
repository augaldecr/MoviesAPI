using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.Services;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Test.UnitTesting
{
    [TestClass]
    public class ActorsControllerTests : TestBase
    {
        [TestMethod]
        public async Task GetPaginatedActors()
        {
            var nameDB = Guid.NewGuid().ToString();
            var context = BuildContext(nameDB);

            context.Actors.Add(new Actor() { Name = "Actor 1" });
            context.Actors.Add(new Actor() { Name = "Actor 2" });
            context.Actors.Add(new Actor() { Name = "Actor 3" });
            await context.SaveChangesAsync();

            var context2 = BuildContext(nameDB);

            var controller = new ActorsController(context2, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var page1 = await controller.GetActor(new PaginationDTO { Page = 1, Qty = 2 });
            var actorsPage1 = page1.Value;
            Assert.AreEqual(2, actorsPage1?.Count());

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var page2 = await controller.GetActor(new PaginationDTO { Page = 2, Qty = 2 });
            var actorsPage2 = page2.Value;
            Assert.AreEqual(1, actorsPage2?.Count());

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var page3 = await controller.GetActor(new PaginationDTO() { Page = 3, Qty = 2 });
            var actorsPage3 = page3.Value;
            Assert.AreEqual(0, actorsPage3?.Count());
        }

        [TestMethod]
        public async Task CreateActorNoPhoto()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            var actor = new ActorCreateDTO() { Name = "Felipe", Birthday = DateTime.Now };

            var mock = new Mock<IFilesStorage>();
            mock.Setup(x => x.SaveFile(null, null, null, null))
                .Returns(Task.FromResult("url"));

            var controller = new ActorsController(context, mock.Object);
            var response = await controller.PostActor(actor);
            var result = response as CreatedAtActionResult;
            Assert.AreEqual(201, result?.StatusCode);

            var context2 = BuildContext(dbName);
            var listado = await context2.Actors.ToListAsync();
            Assert.AreEqual(1, listado.Count());
            Assert.IsNull(listado[0].Photo);

            Assert.AreEqual(0, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task CreateActorWithPhoto()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            var content = Encoding.UTF8.GetBytes("Test image");
            var file = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "image.jpg");
            file.Headers = new HeaderDictionary();
            file.ContentType = "image/jpg";

            var actor = new ActorCreateDTO()
            {
                Name = "New actor",
                Birthday = DateTime.Now,
                Photo = file
            };

            var mock = new Mock<IFilesStorage>();
            mock.Setup(x => x.SaveFile(content, ".jpg", "actors", file.ContentType))
                .Returns(Task.FromResult("url"));

            var controller = new ActorsController(context, mock.Object);
            var response = await controller.PostActor(actor);
            var result = response as CreatedAtActionResult;
            Assert.AreEqual(201, result?.StatusCode);

            var context2 = BuildContext(dbName);
            var list = await context2.Actors.ToListAsync();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("url", list[0].Photo);
            Assert.AreEqual(1, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task PatchReturn404IfNotExist()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            var controller = new ActorsController(context, null);
            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            var response = await controller.PatchActor(1, patchDoc);
            var result = response as StatusCodeResult;
            Assert.AreEqual(404, result?.StatusCode);
        }

        [TestMethod]
        public async Task PatchUpdateAField()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);

            var birthday = DateTime.Now;
            var actor = new Actor() { Name = "Felipe", Birthday = birthday };
            context.Add(actor);
            await context.SaveChangesAsync();

            var context2 = BuildContext(dbName);
            var controller = new ActorsController(context2, null);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;

            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            patchDoc.Operations.Add(new Operation<ActorPatchDTO>("replace", "/name", null, "Claudia"));
            var response = await controller.PatchActor(1, patchDoc);
            var result = response as StatusCodeResult;
            Assert.AreEqual(204, result?.StatusCode);

            var context3 = BuildContext(dbName);
            var actorDB = await context3.Actors.FirstAsync();
            Assert.AreEqual("Claudia", actorDB.Name);
            Assert.AreEqual(birthday, actorDB.Birthday);
        }
    }
}

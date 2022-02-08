using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesAPI.Controllers;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Test.UnitTesting
{
    [TestClass]
    public class MovieTheatersControllerTest : TestBase
    {
        [TestMethod]
        public async Task GetMovieTheaterAt5KMorLess()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                var movieTheaters = new List<Cinema>()
                {
                    new Cinema { Name = "Cinépolis Moravia", Location = geometryFactory.CreatePoint(new Coordinate(-84.05575390254165, 9.962474220277262)) },
                    new Cinema { Name = "Cinépolis Desamparados", Location = geometryFactory.CreatePoint(new Coordinate(-84.06848439993696, 9.8992193523227))},
                    new Cinema { Name = "Cinépolis Paseo de la Flores", Location = geometryFactory.CreatePoint(new Coordinate(-84.110333412203, 9.985486900798657))},
                    new Cinema { Name = "Cinemark Oxígeno", Location = geometryFactory.CreatePoint(new Coordinate(-84.13221938607856, 9.993270429894105))}
                };

                context.AddRange(movieTheaters);
                await context.SaveChangesAsync();
            }

            var filter = new NearestCinemasFilterDTO
            {
                DistanceKms = 5,
                Lat = 9.965646,
                Long = -84.074900
            };

            using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                var controller = new CinemasController(context, geometryFactory);
                var response = await controller.Nearest(filter);
                var value = response.Value;
                Assert.AreEqual(2, value?.Length);
            }
        }
    }
}

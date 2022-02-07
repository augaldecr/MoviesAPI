#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;
using MoviesAPI.Shared.Herlpers;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinemasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly GeometryFactory _geometryFactory;

        public CinemasController(ApplicationDbContext context, 
                                 GeometryFactory geometryFactory)
        {
            _context = context;
            _geometryFactory = geometryFactory;
        }

        // GET: api/Cinemas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CinemaDTO>>> GetCinemas()
        {
            var cinemas = await _context.Cinemas.ToArrayAsync();

            CinemaDTO[] cinemasDTO = cinemas.Select<Cinema, CinemaDTO>(x => x).ToArray();

            return cinemasDTO;
        }

        // GET: api/Cinemas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CinemaDTO>> GetCinema(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);

            if (cinema == null)
            {
                return NotFound();
            }

            CinemaDTO cinemaDTO = cinema;

            return cinemaDTO;
        }

        [HttpGet("closer")]
        public async Task<ActionResult<NearbyCinemaDTO[]>> Closers([FromQuery] NearestCinemasFilterDTO filter)
        {
            var userLocation = _geometryFactory.CreatePoint(new Coordinate(filter.Long, filter.Lat));

            var movieTheaters = await _context.Cinemas.OrderBy(x => x.Location.Distance(userLocation))
                                                      .Where(x => x.Location.IsWithinDistance(userLocation, filter.DistanceKms))
                                                      .Select(x => new NearbyCinemaDTO
                                                      {
                                                          Id = x.Id,
                                                          Name = x.Name,
                                                          Lat = x.Location.Y,
                                                          Long = x.Location.X,
                                                          DistanceMts = Math.Round(x.Location.Distance(userLocation)),
                                                      })
                                                      .ToArrayAsync();

            return movieTheaters;
        }

        // PUT: api/Cinemas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCinema(int id, CinemaCreateDTO cinemaDTO)
        {
            var cinema = await _context.Cinemas.FindAsync(id);

            if (cinema is null)
            {
                return NotFound();
            }

            if (!cinemaDTO.Name.Equals(cinema.Name))
            {
                cinema.Name = cinemaDTO.Name;
            }

            cinema.Location = new GeometryHelper().createPoint(cinemaDTO.Long, cinemaDTO.Lat);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Cinemas
        [HttpPost]
        public async Task<ActionResult<Cinema>> PostCinema(CinemaCreateDTO cinemaCreateDTO)
        {
            Cinema cinema = cinemaCreateDTO;

            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();

            CinemaDTO cinemaDTO = cinema;

            return CreatedAtAction("GetCinema", new { id = cinemaDTO.Id }, cinemaDTO);
        }

        // DELETE: api/Cinemas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCinema(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null)
            {
                return NotFound();
            }

            _context.Cinemas.Remove(cinema);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

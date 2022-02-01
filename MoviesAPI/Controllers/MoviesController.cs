#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Services;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFilesStorage _filesStorage;
        private const string container = "movies";

        public MoviesController(ApplicationDbContext context, IFilesStorage filesStorage) =>
                                                                    (_context, _filesStorage) = (context, filesStorage);

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMovies()
        {
            var movies = await _context.Movies.ToListAsync();
            return movies.Select<Movie, MovieDTO>(x => x).ToArray();
        }

        // GET: api/Movies/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieDTO>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            MovieDTO movieDTO = movie;

            return movieDTO;
        }

        // PUT: api/Movies/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutMovie(int id, [FromForm] MovieCreateDTO movieCreateDTO)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie is null)
                return NotFound();

            if (!string.IsNullOrEmpty(movieCreateDTO.Title) && !movie.Title.Equals(movieCreateDTO.Title))
                movie.Title = movieCreateDTO.Title;

            movie.OnBillboard = movieCreateDTO.OnBillboard;
            
            if (movieCreateDTO.ReleaseDate.Year != 0001 && movie.ReleaseDate != movieCreateDTO.ReleaseDate)
                movie.ReleaseDate = movieCreateDTO.ReleaseDate;

            if (movieCreateDTO.Poster is not null)
            {
                using var memoryStream = new MemoryStream();
                await movieCreateDTO.Poster.CopyToAsync(memoryStream);
                var data = memoryStream.ToArray();
                var extension = Path.GetExtension(movieCreateDTO.Poster.FileName);

                movie.Poster = await _filesStorage.EditFile(data, extension, container, movie.Poster, movieCreateDTO.Poster.ContentType);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> PatchMovie(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            if (patchDocument is null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);

            if (movie is null)
                return NotFound();

            MoviePatchDTO moviePatchDTO = movie;

            patchDocument.ApplyTo(moviePatchDTO, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!string.IsNullOrEmpty(moviePatchDTO.Title) && !moviePatchDTO.Title.Equals(movie.Title))
                movie.Title = moviePatchDTO.Title;

            movie.OnBillboard = moviePatchDTO.OnBillboard;

            if (moviePatchDTO.ReleaseDate.Year != 0001 && moviePatchDTO.ReleaseDate != movie.ReleaseDate)
                movie.ReleaseDate = moviePatchDTO.ReleaseDate;

            _context.Update(movie);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<MovieDTO>> PostMovie([FromForm]MovieCreateDTO movieCreateDTO)
        {
            Movie movie = movieCreateDTO;

            if (movieCreateDTO.Poster is not null)
            {
                using var memoryStream = new MemoryStream();
                await movieCreateDTO.Poster.CopyToAsync(memoryStream);
                var data = memoryStream.ToArray();
                var extension = Path.GetExtension(movieCreateDTO.Poster.FileName);

                movie.Poster = await _filesStorage.SaveFile(data, extension, container, movieCreateDTO.Poster.ContentType);
            }

            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();

            MovieDTO movieDTO = movie;

            return CreatedAtAction("GetMovie", new { id = movieDTO.Id }, movieDTO);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}

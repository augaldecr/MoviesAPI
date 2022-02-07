#nullable disable
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFilesStorage _filesStorage;
        private readonly ILogger<MoviesController> _logger;
        private const string container = "movies";

        public MoviesController(ApplicationDbContext context, 
                                IFilesStorage filesStorage, 
                                ILogger<MoviesController> logger) => 
                                        (_context, _filesStorage, _logger) = (context, filesStorage, logger);

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<MoviesIndexDTO>> GetMovies()
        {
            var top = 5;

            var today = DateTime.Today;

            var futureReleases = await _context.Movies
                    .Where(m => m.ReleaseDate > today)
                    .OrderBy(m => m.ReleaseDate)
                    .Take(top)
                    .ToArrayAsync();

            var onBillboard = await _context.Movies
                    .Where(m => m.OnBillboard)
                    .Take(top)
                    .ToArrayAsync();

            var result = new MoviesIndexDTO
            {
                FutureReleases = futureReleases.Select<Movie, MovieDTO>(x => x).ToArray(),
                OnBillboard = onBillboard.Select<Movie, MovieDTO>(x => x).ToArray(),
            };

            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<MovieDTO[]>> Filter([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(filterMoviesDTO.Title))
                moviesQueryable = moviesQueryable.Where(m => m.Title.Contains(filterMoviesDTO.Title));

            if (filterMoviesDTO.OnBillboard)
                moviesQueryable = moviesQueryable.Where(m => m.OnBillboard);

            if (filterMoviesDTO.FutureReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(m => m.ReleaseDate > today);
            }

            if (filterMoviesDTO.GenreId != 0)
                moviesQueryable = moviesQueryable.Where(m => m.Genres.Select(g => g.Id).Contains(filterMoviesDTO.GenreId));

            if (!string.IsNullOrEmpty(filterMoviesDTO.OrderField))
            {
                var orderType = filterMoviesDTO.AscOrder ? "ascending" : "descending";
                try
                {
                    moviesQueryable = moviesQueryable.OrderBy($"{filterMoviesDTO.OrderField} {orderType}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }

            await HttpContext.InsertParametersPagination(moviesQueryable, filterMoviesDTO.QuantityPerPage);

            var movies = await moviesQueryable.Paginate(filterMoviesDTO.Pagination).ToArrayAsync();
            MovieDTO[] moviesDTO = movies.Select<Movie, MovieDTO>(x => x).ToArray();

            return Ok(moviesDTO);
        }

        // GET: api/Movies/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieDetailsDTO>> GetMovie(int id)
        {
            var movie = await _context.Movies
                                .Include(m => m.ActorMovies) 
                                    .ThenInclude(am => am.Actor)
                                .Include(m => m.Genres)
                                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            MovieDetailsDTO movieDTO = movie;

            return movieDTO;
        }

        // PUT: api/Movies/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutMovie(int id, [FromForm] MovieCreateDTO movieUpdateDTO)
        {
            var movie = await _context.Movies
                                        .Include(m => m.ActorMovies)
                                        .Include(m => m.Genres)
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (movie is null)
                return NotFound();

            //if (movieUpdateDTO.GenresIds is not null )
            //{
            //    Genre[] genres = await _context.Genres.Where(g => movieUpdateDTO.GenresIds.Contains(g.Id)).ToArrayAsync();
            //    if (genres.Select(g => g.Id) != movie.Genres.Select(g => g.Id))
            //    {
            //        movie.Genres = genres;
            //    }
            //}

            if (movieUpdateDTO.GenresIds is not null)
                movie.Genres = await _context.Genres.Where(g => movieUpdateDTO.GenresIds.Contains(g.Id)).ToArrayAsync();

            if (movieUpdateDTO.Actors is not null)
                movie.ActorMovies = movieUpdateDTO.Actors.Select(a => new ActorMovie 
                                                                      { 
                                                                            ActorId = a.ActorId, 
                                                                            Character = a.Character
                                                                       }
                                                         ).ToList();

            if (!string.IsNullOrEmpty(movieUpdateDTO.Title) && !movie.Title.Equals(movieUpdateDTO.Title))
                movie.Title = movieUpdateDTO.Title;

            movie.OnBillboard = movieUpdateDTO.OnBillboard;

            if (movieUpdateDTO.ReleaseDate.Year != 0001 && movie.ReleaseDate != movieUpdateDTO.ReleaseDate)
                movie.ReleaseDate = movieUpdateDTO.ReleaseDate;

            if (movieUpdateDTO.Poster is not null)
            {
                using var memoryStream = new MemoryStream();
                await movieUpdateDTO.Poster.CopyToAsync(memoryStream);
                var data = memoryStream.ToArray();
                var extension = Path.GetExtension(movieUpdateDTO.Poster.FileName);

                movie.Poster = await _filesStorage.EditFile(data, extension, container, movie.Poster, movieUpdateDTO.Poster.ContentType);
            }

            OrderActors(movie);

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
        public async Task<ActionResult<MovieDTO>> PostMovie([FromForm] MovieCreateDTO movieCreateDTO)
        {
            Movie movie = movieCreateDTO;

            movie.Genres = await _context.Genres.Where(g => movieCreateDTO.GenresIds.Contains(g.Id)).ToArrayAsync();

            if (movieCreateDTO.Poster is not null)
            {
                using var memoryStream = new MemoryStream();
                await movieCreateDTO.Poster.CopyToAsync(memoryStream);
                var data = memoryStream.ToArray();
                var extension = Path.GetExtension(movieCreateDTO.Poster.FileName);

                movie.Poster = await _filesStorage.SaveFile(data, extension, container, movieCreateDTO.Poster.ContentType);
            }

            OrderActors(movie);

            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();

            MovieDTO movieDTO = movie;

            return CreatedAtAction("GetMovie", new { id = movieDTO.Id }, movieDTO);
        }

        private void OrderActors(Movie movie)
        {
            if (movie.ActorMovies is not null)
            {
                for (int i = 0; i < movie.ActorMovies.Count; i++)
                {
                    movie.ActorMovies[i].Order = i;
                }
            }
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

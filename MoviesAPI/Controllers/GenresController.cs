#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDTO>>> GetGenres()
        {
            var genres = await _context.Genres.ToListAsync();

            return genres.Select<Genre, GenreDTO>(x => x).ToArray();
        }

        // GET: api/Genres/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GenreDTO>> GetGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            GenreDTO genreDTO = genre;

            return genreDTO;
        }

        // PUT: api/Genres/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutGenre(int id, GenreCreateDTO genreCreateDTO)
        {
            Genre genre = await _context.Genres.FindAsync(id);

            if (genre is null)
                return NotFound();

            genre.Name = genreCreateDTO.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Genres
        [HttpPost]
        public async Task<ActionResult> PostGenre([FromBody] GenreCreateDTO genreCreateDTO)
        {
            Genre genre = genreCreateDTO;
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGenre", new { id = genre.Id }, genre);
        }

        // DELETE: api/Genres/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

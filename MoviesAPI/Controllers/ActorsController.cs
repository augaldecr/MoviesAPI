#nullable disable
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFilesStorage _filesStorage;
        private const string container = "actors";

        public ActorsController(ApplicationDbContext context, IFilesStorage filesStorage)
        {
            _context = context;
            _filesStorage = filesStorage;
        }

        // GET: api/Actors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActorDTO>>> GetActor([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.Actors.AsQueryable();
            await HttpContext.InsertParametersPagination(queryable, paginationDTO.Qty);
            var actors = await queryable.Paginate(paginationDTO).ToListAsync();
            return actors.Select<Actor, ActorDTO>(x => x).ToArray();
        }

        // GET: api/Actors/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActorDTO>> GetActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            ActorDTO actorDTO = actor;

            return Ok(actorDTO);
        }

        // PUT: api/Actors/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutActor(int id, [FromForm] ActorCreateDTO actorDTO)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor is null)
                return NotFound();

            if (!string.IsNullOrEmpty(actorDTO.Name) && !actor.Name.Equals(actorDTO.Name))
                actor.Name = actorDTO.Name;

            if (actor.Birthday != actorDTO.Birthday)
                actor.Birthday = actorDTO.Birthday;

            if (actorDTO.Photo is not null)
            {
                using var memoryStream = new MemoryStream();
                await actorDTO.Photo.CopyToAsync(memoryStream);
                var data = memoryStream.ToArray();
                var extension = Path.GetExtension(actorDTO.Photo.FileName);

                actor.Photo = await _filesStorage.EditFile(data, extension, container, actor.Photo, actorDTO.Photo.ContentType);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> PatchActor(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            if (patchDocument is null)
                return BadRequest();

            var actor = await _context.Actors.FindAsync(id);

            if (actor is null)
                return NotFound();

            ActorPatchDTO actorPatchDTO = actor;

            patchDocument.ApplyTo(actorPatchDTO, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!string.IsNullOrEmpty(actorPatchDTO.Name) && !actorPatchDTO.Name.Equals(actor.Name))
                actor.Name = actorPatchDTO.Name;

            if (actorPatchDTO.Birthday.Year != 0001 && actorPatchDTO.Birthday != actor.Birthday)
                actor.Birthday = actorPatchDTO.Birthday;

            _context.Update(actor);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Actors
        [HttpPost]
        public async Task<ActionResult> PostActor([FromForm] ActorCreateDTO actorDTO)
        {
            Actor actor = actorDTO;

            if (actorDTO.Photo is not null)
            {
                using var memoryStream = new MemoryStream();
                await actorDTO.Photo.CopyToAsync(memoryStream);
                var data = memoryStream.ToArray();
                var extension = Path.GetExtension(actorDTO.Photo.FileName);

                actor.Photo = await _filesStorage.SaveFile(data, extension, container, actorDTO.Photo.ContentType);
            }

            await _context.Actors.AddAsync(actor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActor", new { id = actor.Id }, actor);
        }

        // DELETE: api/Actors/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
    }
}

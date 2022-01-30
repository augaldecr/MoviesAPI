#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Actors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActorDTO>>> GetActor()
        {
            var actors = await _context.Actor.ToListAsync();
            return actors.Select<Actor, ActorDTO>(x => x).ToArray();
        }

        // GET: api/Actors/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActorDTO>> GetActor(int id)
        {
            var actor = await _context.Actor.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            ActorDTO actorDTO = actor;

            return Ok(actorDTO);
        }

        // PUT: api/Actors/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutActor(int id, ActorDTO actorDTO)
        {
            if (id != actorDTO.Id)
            {
                return BadRequest();
            }

            Actor actor = actorDTO;

            _context.Entry(actor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Actors
        [HttpPost]
        public async Task<ActionResult<Actor>> PostActor([FromBody] ActorCreateDTO actorDTO)
        {
            Actor actor = actorDTO;
            await _context.Actor.AddAsync(actor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActor", new { id = actor.Id }, actor);
        }

        // DELETE: api/Actors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            _context.Actor.Remove(actor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }
    }
}

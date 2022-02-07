#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Helpers;
using MoviesAPI.Shared.DTOs;
using MoviesAPI.Shared.Entities;

namespace MoviesAPI.Controllers
{
    [Route("api/movies/{movieId:int}/reviews")]
    [ServiceFilter(typeof(MovieExistAttribute))]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/movies/5/reviews
        [HttpGet]
        public async Task<ActionResult<ReviewDTO[]>> GetReview(int movieId, [FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.Reviews.Include(r => r.User).AsQueryable();
            queryable = queryable.Where(r => r.MovieId == movieId);
            await HttpContext.InsertParametersPagination(queryable, paginationDTO.Qty);
            var reviews = await queryable.Paginate(paginationDTO).ToArrayAsync();
            if (reviews is null)
                return NotFound();
            
            return reviews.Select<Review, ReviewDTO>(x => x).ToArray();
        }

        // PUT: api/Reviews/5
        [HttpPut("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutReview(int reviewId, [FromBody] ReviewCreateDTO reviewCreateDTO)
        {
            var reviewDB = await _context.Reviews.FindAsync(reviewId);

            if (reviewDB is null) { return NotFound(); }

            var userId = HttpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier).Value;

            if (reviewDB.UserId != userId) { return Forbid(); }

            if (!string.IsNullOrEmpty(reviewDB.Comment) && !reviewDB.Comment.Equals(reviewCreateDTO.Comment))
                reviewDB.Comment = reviewCreateDTO.Comment;

            if (reviewDB.Punctuation != reviewCreateDTO.Punctuation)
                reviewDB.Punctuation = reviewCreateDTO.Punctuation;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Reviews
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Review>> PostReview(int movieId, [FromBody]ReviewCreateDTO reviewCreateDTO)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier).Value;

            var reviewExist = await _context.Reviews.AnyAsync(r => r.MovieId == movieId && r.UserId == userId);

            if(reviewExist)
                return BadRequest("The user has already created a review to this movie");

            Review review = reviewCreateDTO;
            review.MovieId = movieId;
            review.UserId = userId;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return NoContent();
            //return CreatedAtAction("GetReview", new { id = review.Id }, review);
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) { return NotFound(); }

            var userId = HttpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier).Value;

            if (review.UserId != userId) { return Forbid(); }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

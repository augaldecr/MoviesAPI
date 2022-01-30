using Microsoft.EntityFrameworkCore;
using MoviesAPI.Shared.Entities;

namespace MoviesAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Actor> Actor { get; set; }
        public DbSet<Genre> Genres { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using MoviesAPI.Shared.Entities;

namespace MoviesAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActorMovie>()
                .HasKey(x => new { x.ActorId, x.MovieId });

            modelBuilder.Entity<ActorMovie>()
                .HasOne(x => x.Actor)
                .WithMany(a => a.ActorMovies)
                .HasForeignKey(x => x.ActorId);

            modelBuilder.Entity<ActorMovie>()
                .HasOne(x => x.Movie)
                .WithMany(m => m.ActorMovies)
                .HasForeignKey(x => x.MovieId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<ActorMovie> ActorsMovies { get; set; }
    }
}

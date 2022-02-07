using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Shared.Entities;
using System.Security.Claims;

namespace MoviesAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
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

            //SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {

            var rolAdminId = "9aae0b6d-d50c-4d0a-9t80-2a6873e3845d";
            var usuarioAdminId = "5673b8cf-12de-48s6-92ad-fae4a77932ad";

            var rolAdmin = new IdentityRole()
            {
                Id = rolAdminId,
                Name = "Admin",
                NormalizedName = "Admin"
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();

            var username = "augaldecr@gmail.com";

            var usuarioAdmin = new IdentityUser()
            {
                Id = usuarioAdminId,
                UserName = username,
                NormalizedUserName = username,
                Email = username,
                NormalizedEmail = username,
                PasswordHash = passwordHasher.HashPassword(null, "Aa123456!")
            };

            modelBuilder.Entity<IdentityUser>()
                .HasData(usuarioAdmin);

            modelBuilder.Entity<IdentityRole>()
                .HasData(rolAdmin);

            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasData(new IdentityUserClaim<string>()
                {
                    Id = 1,
                    ClaimType = ClaimTypes.Role,
                    UserId = usuarioAdminId,
                    ClaimValue = "Admin"
                });
        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<ActorMovie> ActorsMovies { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}

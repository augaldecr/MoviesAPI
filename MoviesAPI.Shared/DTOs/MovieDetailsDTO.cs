namespace MoviesAPI.Shared.DTOs
{
    public record MovieDetailsDTO : MovieDTO
    {
        public GenreDTO[] Genres { get; init; }
        public ActorMovieDetailsDTO[] Actors { get; init; }
    }
}

namespace MoviesAPI.Shared.DTOs
{
    public record ActorMovieCreateDTO
    {
        public int ActorId { get; init; }
        public string Character { get; init; }
    }
}

namespace MoviesAPI.Shared.DTOs
{
    public record ActorMovieDetailsDTO
    {
        public int ActorId { get; init; }
        public string Character { get; init; }
        public string PersonName { get; init; }
    }
}

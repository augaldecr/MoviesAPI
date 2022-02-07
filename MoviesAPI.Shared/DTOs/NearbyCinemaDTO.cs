namespace MoviesAPI.Shared.DTOs
{
    public record NearbyCinemaDTO : CinemaDTO
    {
        public double DistanceMts { get; init; }
    }
}

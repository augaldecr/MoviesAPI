namespace MoviesAPI.Shared.DTOs
{
    public record MoviesIndexDTO
    {
        public MovieDTO[] FutureReleases { get; init; }
        public MovieDTO[] OnBillboard { get; init; }
    }
}

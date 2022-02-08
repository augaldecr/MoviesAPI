using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Shared.DTOs
{
    public record NearestCinemasFilterDTO
    {
        [Range(-90, 90)]
        public double Lat { get; init; }
        [Range(-180, 180)]
        public double Long { get; init; }
        private int distanceKms = 10;
        private int maxDistanceKms = 100;

        public int DistanceKms
        {
            get { return distanceKms; }
            set
            {
                distanceKms = value > maxDistanceKms ? maxDistanceKms : value;
            }
        }

    }
}

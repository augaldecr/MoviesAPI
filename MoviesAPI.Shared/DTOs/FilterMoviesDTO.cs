using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Shared.DTOs
{
    public record FilterMoviesDTO
    {
        public int Page { get; init; } = 1;
        public int QuantityPerPage { get; init; } = 10;

        public PaginationDTO Pagination {   get => new PaginationDTO() { Page = Page, Qty = QuantityPerPage }; }

        public string Title { get; init; }
        public int GenreId { get; init; }
        public bool OnBillboard { get; init; }
        public bool FutureReleases { get; init; }
        public string OrderField { get; init; }
        public bool AscOrder { get; set; }

    }
}

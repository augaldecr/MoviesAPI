using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Shared.DTOs
{
    public record PaginationDTO
    {
        public int Page { get; init; } = 1;

        private int qty = 10;
        private readonly int _maxQtyByPage = 50;

        public int Qty
        { 
            get 
            { 
                return qty; 
            } 
            set 
            {
                qty = value > _maxQtyByPage ? _maxQtyByPage : value; 
            } 
        }
    }
}

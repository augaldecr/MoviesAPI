using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertParametersPagination<T>(this HttpContext httpContext, 
                                                               IQueryable<T> queryable,
                                                               int qtyByPage)
        {
            double qty = await queryable.CountAsync();
            double pagesQty = Math.Ceiling(qtyByPage / qty);

            httpContext.Response.Headers.Add("pagesQty", pagesQty.ToString());
        }
    }
}

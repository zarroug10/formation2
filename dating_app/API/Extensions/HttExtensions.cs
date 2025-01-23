using System;
using System.Text.Json;
using API.Helpers;

namespace API.Extensions;

public  static class HttExtensions
{
     public static void AddPaginationHeader<T> (this HttpResponse response,
     pagedList<T> data )
     {
        var paginationheader = new PaginationHeader(data.CurrentPage, data.PageSize
        , data.TotalCount , data.TotalPages);
         var jsonOptions = 
            new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        response.Headers.Append("Pagination", JsonSerializer
        .Serialize(paginationheader,jsonOptions ));
          response.Headers.Append("Access-Control-Expose-Headers","Pagination");//pagination Access
     }
}

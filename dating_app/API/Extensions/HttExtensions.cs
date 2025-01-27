using System;
using System.Text.Json;
using API.Helpers;

namespace API.Extensions;

public static class HttpExtensions
{
    // A generic method that adds a pagination header to the HTTP response.
    // 'response' is the HTTP response object.
    // 'data' is an instance of PagedList<T>, containing pagination details.
    public static void AddPaginationHeader<T>(this HttpResponse response, pagedList<T> data)
    {
        // Create a pagination header using the pagination details from 'data'.
        var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);

        // Configure JSON serialization options to use camelCase naming.
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Serialize the pagination header and append it to the response headers.
        response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));

        // Allow the 'Pagination' header to be exposed in CORS responses.
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}

using System;

namespace API.Helpers;

public class LikesParams:PaginationParams
{
    public int UserId { get; set; }
    public required string Predecate { get; set; }="liked";
}

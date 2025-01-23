using System;

namespace API.Helpers;

public class UserParms:PaginationParams
{
  
    public string? Gender {get; set;}
    public string? Currentusername {get; set;}
    public int MaxAge {get; set;} = 100;
    public int MinAge {get; set;}= 18;
    public string OrderBy {get; set;} = "lastActive";
    
}

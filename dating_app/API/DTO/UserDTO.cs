using System;

namespace API.DTO;// namespace which is used to make all the files that has the same name sapce visible to each other 

public class UserDTO // User dto file that use to carry the necessary data object to a request
{
    public required string Username { get; set; } // declaration of the username prop
    public required string Token { get; set; } // // declaration of the token prop
    public required string KnownAs { get; set; }
    public required string Gender { get; set; }
    public string? photoUrl { get; set; }
}

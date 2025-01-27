using System;
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities; // namespace makes all the files that has the same name space visble to one another

public class AppUser : IdentityUser<int>
{
    public DateOnly DateOfBirth { get; set; }
    public required string  KnownAs { get; set; }
    public DateTime Created  { get; set; }
    public DateTime LastActive { get; set; }
    public required string Gender { get; set; }
    public string? Introdutrion { get; set; }
    public string? Interests { get; set; }
    public string? LookingFor { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public  List<Photo> Photos { get; set; } =[];
    public List<UserLike> LikedBy{ get; set; } = [];
    public List<UserLike> LikedUsers { get; set; } = [];
    public List<Message> MessagesSent {get; set;} = [];
    public List<Message> MessagesRecieved {get; set;} = [];
    public ICollection<AppUserRole> UserRoles {get; set;}= [];

    public static implicit operator bool(AppUser? v)
    {
        throw new NotImplementedException();
    }
}

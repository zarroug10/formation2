using System;

namespace API.Entities;

public class UserLike
{
    public AppUser SourceUser { get; set; } = null!;
    public int SourceUserId { get; set; }
    public AppUser TorgetUser { get; set; } =null!;
    public int TargetUserId {get; set;}

}

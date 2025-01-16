using System;
using API.Extensions;

namespace API.Entities; // namespace makes all the files that has the same name space visble to one another

public class AppUser
{
    public  int Id { get; set; } // id prop
    public required string UserName { get; set;} // UserName prop
    public  byte[] Passowrdhashed { get; set;} = [];// passwordhashed prop
    public  byte[] PassowrdSalt { get; set;} = [];//  passwordSalt prop
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

    // public int  GetAge (){
    //     return DateOfBirth.CalculateAge();
    // }
}

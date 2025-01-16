using System;

namespace API.DTO;

public class MemberDto
{
    public  int Id { get; set; } // id prop
    public  string? Username { get; set;} 
    public int Age { get; set; }
    public  string?  PhotoUrl { get; set; }
    public  string?  knownAS { get; set; }
    public DateTime Created  { get; set; }
    public DateTime LastActive { get; set; }
    public  string? Gender { get; set; }
    public string? Introdutrion { get; set; }
    public string? Interests { get; set; }
    public string? LookingFor { get; set; }
    public  string? City { get; set; }
    public  string? Country { get; set; }
    public  List<PhotoDTO> Photos { get; set; } 
}

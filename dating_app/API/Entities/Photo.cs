using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities; // namespace makes all the files that has the same name space visble to one another


[Table("Photos")]
public class Photo
{
public int Id { get; set; }
public string Url { get; set; }
public bool IsMain { get; set; }
public string? PublicId { get; set; }

//Naviagtion Property
public int AppUserId { get; set; }
public AppUser AppUser { get; set; } =null!;
}
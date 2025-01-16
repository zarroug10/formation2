using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTO;// namespace is the one that make s all the file on the asme name sapce is visbke to each other 


public class AcountDto //DTO class stand or data transfor Object which is used in methode to carry data through request 
{

[Required]//telling that this data is required without the data cannot be sent 
public  string Username { get; set; } =string.Empty; //declaring te username prop
[Required]
public string?  KnownAs { get; set; }
[Required]
public string?  Gender { get; set; }
[Required]
public string?  DateOfBirth { get; set; }
[Required]
public string?  City { get; set; }
[Required]
public string?  Country { get; set; }

[Required]//telling that this data is required without the data cannot be sent 
[StringLength(12,MinimumLength =8)]// telling that this data should have a length of 12 at max and minimum of 8 .
public  string Password { get; set; } =string.Empty;//declaring the password prop
}

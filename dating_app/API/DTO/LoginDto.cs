using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTO;// name space is used to make t=files that are on the same namesapce visible to each other 

public class LoginDto // a DTO calss that carry the data that gonna be used in the request 
{

    public required string Username { get; set; } // declaration for the username prop


    public required string Password { get; set; }// decalaration for te password prop
}

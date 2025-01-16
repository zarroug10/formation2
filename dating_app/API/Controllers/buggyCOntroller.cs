using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers; // namesace which is used to make the files on the same namespace vsisible to each other

public class buggyCOntroller (DataContext context):BaseApiCOntroller // the controller using the dependency injection to inject the dataContext
{
    [Authorize] // used to make a protected end point
    [HttpGet("auth")] // a http get request with the rout /auth
     public ActionResult<string> Getauth(){ // a simple function action resuklt with a return type string 
        return "Seceret key"; // return thee secret key if the it's auth which is not so the result would be Unautherized 
     }
     
      [HttpGet("Not-Found")]  // http get request with the route /Not-found
     public ActionResult<AppUser> GetNotFound(){//an actionresultfunction with return type of AppUser 
        var  example = context.Users.Find(-1); // a data holder for  a user with a -1 user id
        if (example == null) // if the user doessn't exist the user is not found
        return NotFound();
        return example  ; // return the requested user 
     }

      [HttpGet("Server-error")] // http get with rout //Server-error
     public ActionResult<AppUser> GetServerError(){ //an action result function with the return type of AppUser
         var  example = context.Users.Find(-1) ?? throw new  Exception("bad request") ; // a data holder of a user with the a -1 user id if it's null throw an Exception
         return example ;
       }

      [HttpGet("bad-request")] // a Httpget request with the rout /bad-request
     public ActionResult<string> GetBadrequest(){ //an actionresukt function with a string return type 
        return BadRequest("invalid requests"); // it just return a Bad request
     }
}

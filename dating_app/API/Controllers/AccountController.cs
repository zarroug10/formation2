using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;// name space is valuable to make the files with similar name space visble to each other 

public class AccountController(DataContext context,
ITokenService tokenService, ILogger<AccountController> logger, IMapper mapper) : BaseApiCOntroller // a clean code with an easier constructor which we used in he dependency injection 
{


    [HttpPost("register")]//http post with the //register post
    public async Task<ActionResult<UserDTO>> Register(AcountDto accountDto)
    {//an async function for registering the user that return a UserDTO type and uses the accountDto to send requests
        if (await UserExists(accountDto.Username))
        {
            logger.LogError("Username is already in use");// similar to the console log in node js
            return Unauthorized("Username Exists");// a conditional with a methode to check if the username exists then it returns  UNautherized

        }
        using var hmac = new HMACSHA512(); //delcartiion of the hmac to encrypte the  users passsword

        var user = mapper.Map<AppUser>(accountDto);

        user.UserName = accountDto.Username.ToLower();
        user.Passowrdhashed = hmac.ComputeHash(Encoding.UTF8.GetBytes(accountDto.Password));
        user.PassowrdSalt = hmac.Key ;

        //  var user = new AppUser { //storing the user that will be created with the new instance initialization of the user entities
        //     UserName = accountDto.Username.ToLower() ,
        //     Passowrdhashed = hmac.ComputeHash(Encoding.UTF8.GetBytes(accountDto.Password)),//hashed password is defined by the combination of the given pwd  and the key wihich is here is PassowrdSalt
        //     PassowrdSalt= hmac.Key
        //  };
        context.Add(user);//Add the user (track the new given user and will be inserted into the database when  calling the saveChangesAsynch)
        await context.SaveChangesAsync(); // to save the user into, the database 
        return new UserDTO
        { //return the user with its username and  token
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender,// it uses the token service interface to create the token
        };
    }


    [HttpPost("login")]//http request with route /login
    public async Task<ActionResult<UserDTO>> Login(LoginDto loginDto)
    { //login funcrtion with a UserDtO type value 
        var user = await context.Users
        .Include(p => p.Photos)
        .FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());//to test retuning a user having the same Username with firstOrDefault returning matching data 
        if (user == null) return Unauthorized("Invalid username");// if he doesn't exist exist tehen it is not authorized

        using var hmac = new HMACSHA512(user.PassowrdSalt);//HMAC stand for Hash-based Message Authentication Code .. initialized with the key password salt touse the existing user's passwordSalt

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)); //we used the hmac to  to cobine our login password with PasswordSalt and hash them 

        for (int i = 0; i < computedHash.Length; i++)// then compare them to the user's passwrd hashed 
        {
            if (computedHash[i] != user.Passowrdhashed[i]) return Unauthorized("Invalid Password"); // if it's the right pwd the user is logged in else Unauthorized
        }
        return new UserDTO
        {
            Username = user.UserName, // then return the user with his username and given token
            Token = tokenService.CreateToken(user),// this token is created using the interface of the token service that has a create token methode with params 
            KnownAs = user.KnownAs,
            Gender = user.Gender,
            photoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    private async Task<bool> UserExists(string username)
    {// a methode  to check of the username exists
        return await context.Users.AnyAsync(u => u.UserName.ToLower() == username);// here is using the anyasync element to check if the element Username exisit or not 
    }


}

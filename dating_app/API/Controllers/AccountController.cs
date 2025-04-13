using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;


using API.DTO;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;// name space is valuable to make the files with similar name space visble to each other 
[AllowAnonymous]
public class AccountController(UserManager<AppUser> userManager,
ITokenService tokenService,
 ILogger<AccountController> logger, IMapper mapper) : BaseApiCOntroller // a clean code with an easier constructor which we used in he dependency injection 
{


    [HttpPost("register")]//http post with the //register post
    public async Task<ActionResult<UserDTO>> Register(AcountDto accountDto)
    {//an async function for registering the user that return a UserDTO type and uses the accountDto to send requests
        if (await UserExists(accountDto.Username))
        {
            logger.LogError("Username is already in use");// similar to the console log in node js
            return Unauthorized("Username Exists");// a conditional with a methode to check if the username exists then it returns  UNautherized

        }

        var user = mapper.Map<AppUser>(accountDto);

        user.UserName = accountDto.Username.ToLower();
        var results = await userManager.CreateAsync(user,accountDto.Password);
        if(!results.Succeeded) return BadRequest(results.Errors);
        return new UserDTO
        { //return the user with its username and  token
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender,// it uses the token service interface to create the token
        };
    }


    [HttpPost("login")]//http request with route /login
    public async Task<ActionResult<UserDTO>> Login(LoginDto loginDto)
    { //login funcrtion with a UserDtO type value 
        var user = await userManager.Users
        .Include(p => p.Photos)
        .FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.Username.ToUpper());//to test retuning a user having the same Username with firstOrDefault returning matching data 
        
        if (user == null || user.UserName == null) return Unauthorized("Invalid username");// if he doesn't exist exist tehen it is not authorized

        var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid) return Unauthorized("Invalid password");
        user.LastLogin = DateTime.Now;
        await userManager.UpdateAsync(user);

        return new UserDTO
        {
            Username = user.UserName, // then return the user with his username and given token
            Token = await tokenService.CreateToken(user),// this token is created using the interface of the token service that has a create token methode with params 
            KnownAs = user.KnownAs,
            Gender = user.Gender,
            photoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            LastLogin = user.LastLogin
        };
    }

    private async Task<bool> UserExists(string username)
    {// a methode  to check of the username exists
        return await userManager.Users.AnyAsync(u => u.NormalizedUserName == username.ToUpper());// here is using the anyasync element to check if the element Username exisit or not 
    }
}

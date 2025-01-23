using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services; // name spaceto prevent the naming conflict between the files 

public class TokenService(IConfiguration config, UserManager<AppUser> userManager) : ITokenService // class that uses the interface of the configuration using dependedncy injection and the inherentce to use the interface 
// of the token interface
{
    public async Task<string> CreateToken(AppUser user) //the methode inherted from the token interface 
    {
        var tokenkey = config["TokenKey"] ?? throw new Exception("Cannot Access token key from appsettings"); //hceking and validating the token key 
        if (tokenkey.Length < 64) throw new Exception(" Yoour token key needs to be longer");// conition for the length of the token  key where it should be less than 64

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenkey));//a data holder for the symetric security key that is used to encryption and validation 
                                                                             //and for th easssword to be understood we are using the tokenkey

        if (user.UserName == null) throw new Exception("No Username For User");
        var claim = new List<Claim>  // defining what  calim contains 
        {
           new(ClaimTypes.Name, user.UserName),
           new(ClaimTypes.NameIdentifier, user.Id.ToString()) //the declartationg the USernale that wll be cntaining the claim with claimType  Name Identifier
        };
        
        var roles = await userManager.GetRolesAsync(user);
        claim.AddRange(roles.Select(role => new Claim(ClaimTypes.Role ,role)));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);  // initilazing the signing credentials  and hasing them using th ekey which is the 
        //encrypted validated token key and  addding to it the unique hashed signeture 
        var tokenDescriptor = new SecurityTokenDescriptor //the descriptor holds the subject , expiring date and the signing credentials 
        {
            Subject = new ClaimsIdentity(claim),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler(); //new instance of the jwtscuritytoken handler  for validation and creatong json web token
        var token = tokenHandler.CreateToken(tokenDescriptor); // the data holder of the created token ... using the createtoken extended from the token handler
        return tokenHandler.WriteToken(token); // retruning the created token using the write token extnded from the token handler 
    }
}

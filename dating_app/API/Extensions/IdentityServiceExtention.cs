using System;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using API.Data;
using API.Entities;

namespace API.Extensions;// name sapce that makes all the files containing it visible to one another

public  static class IdentityServiceExtention 
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config) // Static class that uses the config and service interfaces to use the service of add
    // authentifaction and add the token key to the configurations
    {
       services.AddIdentityCore<AppUser>(opt =>{
            opt.Password.RequireNonAlphanumeric = false ;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();

       services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme). // add authetifcation to the servic collection using th jwt token scheme
        AddJwtBearer(// we are coonfiguring the jwt token options
                options =>
            {
                var tokenKey = config["TokenKey"] ?? throw new Exception("Token not found"); // here is to check and retraived from the configuaration for the token key labled TokenKey if it's null
                options.TokenValidationParameters = new TokenValidationParameters // sets the token validation params
                {
                    ValidateIssuerSigningKey = true,// ensure that the token signing key is valid
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)), // setting the signing key for the validating the token
                    ValidateIssuer = false,// disable validation for the issuer 
                    ValidateAudience = false,// disable validation for the audiance
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken ;
                        }
                        return Task.CompletedTask;
                    }
                };
            }
        );

        services.AddAuthorizationBuilder()
        .AddPolicy("RequireAdminRole",policy => policy.RequireRole("Admin"))
        .AddPolicy("ModeratePhotoRole",policy => policy.RequireRole("Admin","Moderator"));

        return services;
    }
}

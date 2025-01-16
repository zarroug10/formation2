using System;
using API.Data;
using API.Helpers;
using API.interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;// name sapce that makes all the files containing it visible to one another

public static class Application_ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) // the iservice collection has the addAplicationService as an extension that has the 
    //services and the cifiguration 
    {
        services.AddControllers();//adding the controllers
       services.AddDbContext<DataContext>(op =>//adding the dbcontext with return type of datacontext class with op represtion the options
        {
            op.UseSqlite(config.GetConnectionString("DefaultConnection"));//options using squilte providers
        });
        services.AddCors(); // adding the cors 
        services.AddScoped<ITokenService, TokenService>(); // we are using the addScoped to add the Itoken service token service to it.so they be reused throughout the life time of the scope
        services.AddScoped<IUserRepository, UserRepository>(); 
        services.AddScoped<IPhotoService, PhotoService>(); 
        services.AddScoped<LogUserActivity>(); 
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings")); 
        
        return  services;
    }
}

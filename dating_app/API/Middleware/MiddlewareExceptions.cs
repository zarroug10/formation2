using System;
using System.Net;
using System.Text.Json;

using Microsoft.OpenApi.Exceptions;

using API.Errors;

namespace API.Middleware; // name sapce to  prevent naming conflicts

public class MiddlewareExceptions(RequestDelegate next ,ILogger<MiddlewareExceptions> logger ,IHostEnvironment env ) // class with the ijection of the requestdeleget and logger and hostenvironment 
{
    public async Task InvokeAsync(HttpContext context) // invoking thhe http context at the sapcified proiority 
    {
        try
        {
            await next(context);// waiting for the next request
        }
        catch (Exception ex) // cathcing the exceptions which is here the errors 
        {
            logger.LogError(ex,ex.Message); // logger here is used to display the error message
            context.Response.ContentType = "application/json"; // decalring the type of the content Type of the response 
            context.Response.StatusCode= (int)HttpStatusCode.InternalServerError; // decalring the Status code of response 

            var response = env.IsDevelopment() // place holder for the environment of the host if it's in dev
            ? new Exceptions(context.Response.StatusCode, ex.Message, ex.StackTrace)  // if it's in dev env then return this tye of exception 
            : new (context.Response.StatusCode,ex.Message ,"Internal Server Error");// else return it with this type of the exception

            var options = new JsonSerializerOptions //initialization of the JsonseriliazerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // declaring the naming policy of the property as Camel case
            };
            var json = JsonSerializer.Serialize(response,options); //w are serilizing the resopnse into a json option using the specified options

            await context.Response.WriteAsync(json);//send the json string as the response to the client
        }
    }
}

using System;
using API.Extensions;
using API.interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

// class that inherits from IAsyncActionFilter A filter that asynchronously surrounds execution of the action, after model binding is complete.
public class LogUserActivity : IAsyncActionFilter 
{
    // an async task to excute on action that take as 
    //<params>"context"As the context for action filters,</params> 
    //<params>"next" A Task that on completion returns an ActionExecutedContext.</params> 
    //<summery>updates the last active property every time a specified user do an action 
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {

        // variable that takes result of an ActionExecutedContext
        var resultsContext = await next();

        //after the action has been excuted check if the user is authenticated if he is not return
        if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

        //userid variable to hold the user's id 
        var userId = resultsContext.HttpContext.User.GetUserId();

        // a repo variable to hold the  IUserRepository 
        var repo = resultsContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();

        // using te userId the user variabke holds the data of the user 
        var user = await repo.UserRepository.GetUserByIdAsync(userId);

        // if the user is null return
        if (user == null) return;
        
        // update the user Last active prop to the current datetime
        user.LastActive = DateTime.UtcNow;

        //save the updated Last active to database 
        await repo.Complete();
    }
}

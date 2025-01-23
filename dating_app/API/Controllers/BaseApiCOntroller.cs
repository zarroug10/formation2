using System;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers; //the name space is essential to make the files visible to each other

[Authorize]
[ServiceFilter(typeof(LogUserActivity))]
[ApiController]// the type of the class we are creating (controller) ,ad it's essential to call it when creating a controller
[Route("api/[controller]")] // the route with the url /api/{name of the controller} whoc is here a dynamic one 
public class BaseApiCOntroller : ControllerBase // inherentence: the class is inherenting the  controllerBase which required in every controller
{

}

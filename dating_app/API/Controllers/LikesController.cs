using System;

using Microsoft.AspNetCore.Mvc;

using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;

namespace API.Controllers;

public class LikesController(IUnitOfWork unitOfWork) : BaseApiCOntroller
{
    [HttpPost("{torgetUserId:int}")]

    // an async Task with the action result data type  that adds and remove the like on the user by toggling
    public async Task<ActionResult> ToggleLike(int torgetUserId)
    {
        // source user id is the logged in user id(currentuser)
        var sourceUserId = User.GetUserId();

        // if the sourceUserId is equal to the torget user than we return a bad request
        if (sourceUserId == torgetUserId) return BadRequest("You cannot like Yourself");

        //existingLike variable that holds of the like  that exist at the target user and the source user ids
        var existingLike = await unitOfWork.LikesRepository.GetUserLikeint(sourceUserId, torgetUserId);
        //if the existingLike variable is null then we add a new like with the target userid and source user id
        //else we delete the existingLike .
        if (existingLike == null)
        {
            var like = new UserLike
            {
                TargetUserId = torgetUserId,
                SourceUserId = sourceUserId
            };
            unitOfWork.LikesRepository.AddLike(like);
        }
        else
        {
            unitOfWork.LikesRepository.DeleteLike(existingLike);
        }
        // if there is no errors save chnages else we return a bad request
        if (await unitOfWork.Complete()) return Ok();
        return BadRequest("failed to update like");
    }

    [HttpGet("list")]

    // an async Task with the action result data type and Ienumrable for iteration with int datatype that returns the current user like ids
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(await unitOfWork.LikesRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }

    [HttpGet]
    // Asynchronous action method that returns a list of liked users.
    // Returns an IEnumerable of MemberDto wrapped in an ActionResult.
    // <summary> Retrieves users liked by the current user based on the provided LikeParams. </summary>
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        // Assign the current user's ID to the LikeParams object.
        likesParams.UserId = User.GetUserId();

        // Fetch the liked users from the repository.
        var users = await unitOfWork.LikesRepository.GetUserLikes(likesParams);

        // Add pagination headers to the response.
        Response.AddPaginationHeader(users);

        // Return the list of liked users.
        return Ok(users);
    }
}

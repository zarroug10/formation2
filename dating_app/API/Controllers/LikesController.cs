using System;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepository likesRepository) : BaseApiCOntroller
{
    [HttpPost("{torgetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int torgetUserId)
    {
        var sourceUserId = User.GetUserId();

        if (sourceUserId == torgetUserId) return BadRequest("You cannot like Yourself");

        var existingLike = await likesRepository.GetUserLikeint(sourceUserId, torgetUserId);
        if (existingLike == null)
        {
            var like = new UserLike
            {
                TargetUserId = torgetUserId,
                SourceUserId = sourceUserId
            };
            likesRepository.AddLike(like);
        }
        else
        {
            likesRepository.DeleteLike(existingLike);
        }
        if (await likesRepository.SaveChanges()) return Ok();
        return BadRequest("failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await likesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(users);
        return Ok(users);
    }

}

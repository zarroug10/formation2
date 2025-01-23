using System;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager): BaseApiCOntroller
{
    [Authorize(Policy ="RequireAdminRole")]
    [HttpGet("users-with-admin")]
    public  async Task<ActionResult> GetUsersWithRoles ()
    {
        var users = await userManager.Users
        .OrderBy(x => x.UserName)
        .Select(x=> new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
        }
        ).ToListAsync();
        return Ok(users);
    }

        [Authorize(Policy ="ModeratePhotoRole")]
    [HttpGet("users-with-adminModer")]
    public  async Task<ActionResult> GetPhotoForModeration ()
    {
        var users = await userManager.Users
        .OrderBy(x => x.UserName)
        .Select(x=> new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
        }
        ).ToListAsync();
        return Ok(users);
    }

    [Authorize(Policy ="ModeratePhotoRole")]
    [HttpGet("users-with-adminModer/{username}")]
    public async Task<ActionResult> GetUserWithRolesByUsername( string username)
    {
     var user = await userManager.Users
        .Where(x => x.UserName == username)
        .Select(x=> new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
        }
        ).SingleOrDefaultAsync();

        return Ok(user);
    }

    [Authorize(Policy ="RequireAdminRole")]
    [HttpPost("editRole/{username}")]
    public  async Task<ActionResult> EditRoles ( string username,string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");

        var selectedRoles= roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("User Not Found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles));

        if(!result.Succeeded) return BadRequest("Failed to Remove from Roles");

        return Ok( await userManager.GetRolesAsync(user));
    }

}

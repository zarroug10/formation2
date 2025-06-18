using System;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using API.DTO;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Services;
using API.interfaces;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService) : BaseApiCOntroller
{
    // Authorized with the policy of only  Admin Role are required and allowed to access this data 
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-admin")]

    // an async func that have an action result as its data output to get users with their roles
    public async Task<ActionResult> GetUsersWithRoles()
    {
        // declaring a users variable that take a list of users orderd by username and selected only their username , id and roles to be displayed 
        var users = await userManager.Users
        .OrderBy(x => x.UserName)
        .Select(x => new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()// display the roles in a list and only select the Name
        }
        ).ToListAsync();
        return Ok(users);
    }

    //Authorized with the policy of ModeratePhotoRole
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("users-with-adminModer")]

    //an async function that have an action result as it's data type 
    public async Task<ActionResult> GetPhotoForModeration()
    {
        // users variable to get users orderd by username and select only their username,id and roles
        var users = await userManager.Users
        .OrderBy(x => x.UserName)
        .Select(x => new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()// display the roles in a list and only select the Name
        }
        ).ToListAsync();
        return Ok(users);
    }

    // AUthorized api with the ModeratePhotoRole policy 
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("users-with-adminModer/{username}")]

    //an async func that have an action result as it's data type 
    public async Task<ActionResult> GetUserWithRolesByUsername(string username)
    {
        //user variable that takes the users and it's Username value is equal to username  params and then only select the id username and role and display only one
        var user = await userManager.Users
           .Where(x => x.UserName == username)
           .Select(x => new
           {
               x.Id,
               Username = x.UserName,
               Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
           }
           ).SingleOrDefaultAsync();

        return Ok(user);
    }

    // AUthorized api with the RequireAdminRole policy 
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("editRole/{username}")]

    // an async function with an action result data type to edit the roles of users 
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        // if the roles params is empty return BadRequest
        if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");

        // selectedRoles variable that splits the role with the comma and display them in an array []
        var selectedRoles = roles.Split(",").ToArray();

        //user variable that gets the user by his username
        var user = await userManager.FindByNameAsync(username);

        // if the user is null return a BadRequest
        if (user == null) return BadRequest("User Not Found");

        //userRoles variable that Gets a list of role names the specified user belongs to
        var userRoles = await userManager.GetRolesAsync(user);

        // a result variable that Adds the selectedRoles only the selected roles to the user and prevent the adding of the same userrole to the same user using .expect 
        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));//expect take list1 and list2 and onnly return list1 sequences that is not in list2

        // if the result is failed return a bad request
        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        //a result variable that removes roles from the sepecified user and only return the sequences of userRoles that is not in selectedRoles
        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));//expect take list1 and list2 and onnly return list1 sequences that is not in list2

        //if the result is failed return a bad request
        if (!result.Succeeded) return BadRequest("Failed to Remove from Roles");

        //Get a Ok result with the list of user roles
        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet]
    public async Task<ActionResult<PhotoForApprovalDto>> Getphotos()
    {
        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();

        return Ok(photos);
    }
    
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        // Attempt to retrieve the photo from the repository by its ID.
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        // If the photo does not exist in the database, return a BadRequest response with an error message.
        if (photo == null) return BadRequest("Could not get photo from db");

        // Check if the photo has a PublicId (indicating it's stored in an external service, like cloud storage).
        if (photo.PublicId != null)
        {
            // Call the photo service to delete the photo from the external service using the PublicId.
            var result = await photoService.DeletePhotoAsync(photo.PublicId);

            // If the deletion from the external service was successful ,
            // remove the photo from the local database repository.
            if (result.Result == "ok")
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
        }
        else
        {
            // If the photo does not have a PublicId ,
            // simply remove it from the database.
            unitOfWork.PhotoRepository.RemovePhoto(photo);
        }

        // Commit the changes to the database, ensuring the photo is deleted both locally and externally (if applicable).
        await unitOfWork.Complete();

        // Return an Ok response to confirm the operation was successful.
        return Ok();
    }


    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        // Attempt to retrieve the photo from the repository by its ID.
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        // If the photo does not exist in the database, return a BadRequest response with an error message.
        if (photo == null) return BadRequest("Could not get photo from db");

        photo.IsApproved = true;

        var user = await unitOfWork.UserRepository.GetUserByPhotoId(photoId);
        if (user == null) return BadRequest("Could not get user from db");
        if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;
        await unitOfWork.Complete();
        return Ok();
    }


}

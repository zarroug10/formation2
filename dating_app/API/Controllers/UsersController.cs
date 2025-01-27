using System;
using System.Security.Claims;
using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers; //name space is valuable for creating a connected files without the name space the file is not visivble toeach other



[Authorize]
public class UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService) : BaseApiCOntroller //here we re using the encapsuklation where we created a basedapicontroller 
{


   [HttpGet] //get
   public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParms userParams)
   { //function for retreaving the list of users with the async 

      userParams.Currentusername = User.GetUsername();
      var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);//to Listasync is to create a list of the datta retreaved

      Response.AddPaginationHeader(users);

      return Ok(users);
   }

   [HttpGet("{id:int}")] //get wuth the routing id as it's dynamic route
   public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserbyId(int id)
   { //methode to get a specofc user by it's id 
      var user = await unitOfWork.UserRepository.GetUserByIdAsync(id);//findasync is to finds an entity with the given key value (id) and it can return return the entity immediatly if the it's already teacked by the context
      if (user == null) return NotFound(); // if the user is null the result would be Notfound (404)


      return Ok(user);//else the user data is returned (200)
   }


   [HttpGet("{username}")] //get wuth the routing id as it's dynamic route
   public async Task<ActionResult<MemberDto>> GetUserbyUsername(string username)
   {
      var currentUsername = User.GetUsername();
      // Attempt to retrieve user information from the repository
      var user = await unitOfWork.UserRepository.GetMemberAsync(username,
      isCurrentUser: currentUsername == username);

      // If user is not found, return a 404 Not Found response
      if (user == null)
         return NotFound();

      // Return the found user information
      return user;
   }

   [HttpPut]
   // Asynchronous action method that updates the current user.
   // <summary> Updates the current user's information. </summary>
   public async Task<ActionResult> UpdateUser(MemberUpadteDTO member)
   {
      // Retrieve the user from the repository using their username.
      var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

      // Return a bad request if the user is not found.
      if (user == null) return BadRequest("User not found");

      // Map the updated member data to the user entity.
      mapper.Map(member, user);

      // Save changes to the repository and return NoContent if successful.
      if (await unitOfWork.Complete()) return NoContent();

      // Return a bad request if the update operation fails.
      return BadRequest("Failed to update the user");
   }

   [HttpPost("add-photo")]
   public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
   {
      var user = await
     unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
      if (user == null) return BadRequest("Cannot update user");
      var result = await photoService.AddPhotoAsync(file);

      if (result.Error != null) return BadRequest(result.Error.Message);
      var photo = new Photo
      {
         Url = result.SecureUrl.AbsoluteUri,
         PublicId = result.PublicId
      };
      user.Photos.Add(photo);
      if (await unitOfWork.Complete())
         return CreatedAtAction(nameof(GetUserbyUsername),
         new { username = user.UserName }, mapper.Map<PhotoDTO>(photo));
      return BadRequest("Problem adding photo");
   }


   [HttpPut("set-Photo/{photoId:int}")]
   // Asynchronous action method to set the main photo for the current user.
   // Returns an ActionResult.
   // <summary>Sets te main photo for the current user</summary>
   public async Task<ActionResult> setMain(int photoId)
   {
      // Retrieve the user from the repository using their username.
      var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

      //returns a BAd Request if the user is not Found
      if (user == null) return BadRequest("No user Has been Found");

      //Retrieve the user's photo using the photo id
      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      // returns a Bad Request if either the photo is not found or the photo is already the main 
      if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");

      //Retrieves the current user main phtot 
      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

      //set the new photo to main and remove the current main photo
      if (currentMain != null) currentMain.IsMain = false;
      photo.IsMain = true;

      //If saving the user to the repository was successful return NoContent.
      if (await unitOfWork.Complete()) return NoContent();
      return BadRequest("Failed to Update the main Photo");
   }

   [HttpDelete("delete-photo/{photoId:int}")]
   public async Task<ActionResult> DeletePhoto(int photoId)
   {
     var user = await
     unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
      if (user == null) return BadRequest("User not found");
      var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);
      if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");
      if (photo.PublicId != null)
      {
         var result = await photoService.DeletePhotoAsync(photo.PublicId);
         if (result.Error != null) return BadRequest(result.Error.Message);
      }
      user.Photos.Remove(photo);
      if (await unitOfWork.Complete()) return Ok();
      return BadRequest("Problem deleting photo");
   }
}

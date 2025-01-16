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
using Microsoft.EntityFrameworkCore;

namespace API.Controllers; //name space is valuable for creating a connected files without the name space the file is not visivble toeach other



[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) : BaseApiCOntroller //here we re using the encapsuklation where we created a basedapicontroller 
{
   [HttpGet] //get
   public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParms userParams)
   { //function for retreaving the list of users with the async 

      userParams.Currentusername = User.GetUsername();
      var users = await userRepository.GetMembersAsync(userParams);//to Listasync is to create a list of the datta retreaved

      Response.AddPAgincationHeader(users);

      return Ok(users);
   }

   [Authorize]//protected api
   [HttpGet("{id:int}")] //get wuth the routing id as it's dynamic route
   public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserbyId(int id)
   { //methode to get a specofc user by it's id 
      var user = await userRepository.GetUserByIdAsync(id);//findasync is to finds an entity with the given key value (id) and it can return return the entity immediatly if the it's already teacked by the context
      if (user == null) return NotFound(); // if the user is null the result would be Notfound (404)


      return Ok(user);//else the user data is returned (200)
   }

   [Authorize]//protected api
   [HttpGet("{username}")] //get wuth the routing id as it's dynamic route
   public async Task<ActionResult<MemberDto>> GetUserbyUsername(string username)
   {
      // Attempt to retrieve user information from the repository
      var user = await userRepository.GetMemberAsync(username);

      // If user is not found, return a 404 Not Found response
      if (user == null)
         return NotFound();

      // Return the found user information
      return user;
   }

   [HttpPut]
   public async Task<ActionResult> UpdateUser(MemberUpadteDTO member)
   {
      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
      if (user == null) return BadRequest("User is not found");

      mapper.Map(member, user);
      if (await userRepository.SaveAllAsync()) return NoContent();
      return BadRequest("Failed to Update the user");
   }

   [HttpPost("add-photo")]
   public async Task<ActionResult<PhotoDTO>> UploadPhoto(IFormFile file)
   {
      if (file == null) return BadRequest("No file provided");

      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
      if (user == null) return BadRequest("No user Has been Found");

      var results = await photoService.AddPhotoAsync(file);
      if (results.Error != null) return BadRequest(results.Error.Message);


      var photo = new Photo
      {
         Url = results.SecureUrl.AbsoluteUri,
         PublicId = results.PublicId,
      };
      if (user.Photos.Count == 0) photo.IsMain = true;


      user.Photos.Add(photo);

      if (await userRepository.SaveAllAsync())
         return CreatedAtAction(nameof(GetUserbyUsername),
         new { username = user.UserName }, mapper.Map<PhotoDTO>(photo));
      return BadRequest("Error while Uploading the photo");
   }


   [HttpPut("set-Photo/{photoId:int}")]

   public async Task<ActionResult> setMain(int photoId)
   {
      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
      if (user == null) return BadRequest("No user Has been Found");
      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
      if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");
      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
      if (currentMain != null) currentMain.IsMain = false;
      photo.IsMain = true;
      if (await userRepository.SaveAllAsync()) return NoContent();
      return BadRequest("Failed to Update the main Photo");
   }

   [HttpDelete("Delete-photo/{photoId}")]

   public async Task<ActionResult> DeletePhoto(int photoId)
   {
      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
      if (user == null) return BadRequest("No user Has been Found");
      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
      if (photo == null || photo.IsMain) return BadRequest("this photo cannot be deleted");


      if (photo.PublicId != null)
      {
         var results = await photoService.DeletePhotoAsync(photo.PublicId);
         if (results.Error != null) return BadRequest(results.Error.Message);
      };
      user.Photos.Remove(photo);
      if (await userRepository.SaveAllAsync()) return Ok();
      return BadRequest("Problem Deleting Photo");
   }
}

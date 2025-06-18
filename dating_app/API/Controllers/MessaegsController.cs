using System;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;

namespace API.Controllers;


[Authorize]
public class MessagesController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiCOntroller
{

  [HttpPost]
  public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
  {

    //Get the user's Username
    var username = User.GetUsername();
    //if the username is the same as the recipient username return a bad request
    if (username == createMessageDTO.RecipientUsername.ToLower())
      return BadRequest("You can not send Messages to yourself");
    // the value if the send is the user's username  
    var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
    //the value of the recipient is the recipient username
    var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

    // if either the sender or his username  or the recipient or his username is null return a bad request
    if (sender == null || recipient == null || sender.UserName == null  ||recipient.UserName == null  )
    {
      return BadRequest("You can not send Messages");
    }
    //Create a new message 
    var message = new Message
    {
      Sender = sender,
      Recipient = recipient,
      SenderUsername = sender.UserName,
      RecipientUsername = recipient.UserName,
      Content = createMessageDTO.Content
    };
    //Add the message 
    unitOfWork.MessageRepository.AddMessage(message);
    //If the message is Saved return and ok and map the message into the messagedto else return a bad request
    if (await unitOfWork.Complete()) return Ok(mapper.Map<MessageDTO>(message));
    return BadRequest("an error has occured connot send message");
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery]
     MessageParams messageParams)
  {
    //Get the username as the message Params username
    messageParams.Username = User.GetUsername();
    //Get the message using the message params 
    var message = await unitOfWork.MessageRepository.GetMessagesforUser(messageParams);
    //Add pagination header to the message 
    Response.AddPaginationHeader(message);
    //return the message 
    return message;
  }

  [HttpGet("thread/{username}")]
  public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
  {
    //Get the Current user username 
    var currentUsername = User.GetUsername();
    //return the message thread through the params of the current username and username 
    return Ok(await unitOfWork.MessageRepository.GetMessageThread(currentUsername, username) );
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteMessage(int id)
  {
    //get the user's username from the token
    var Username = User.GetUsername();
    //get the message through it's id
    var message = await unitOfWork.MessageRepository.GetMessaegs(id);
    //if the message is null return a bad request
    if(message == null) return BadRequest("Cannot delete this message");

    //if the usernames of the sender and the recipient are different from the current username return forbidden
    if(message.SenderUsername != Username && message.RecipientUsername != Username ) 
    return Forbid();
    //if the sender's username is the same as the currentusernme turn SenderDeleted to true 
    if (message.SenderUsername == Username) message.SenderDeleted =true ;
    //if the recipient's username is  the same as the currentusernme turn RecipientDeleted to true 
    if (message.RecipientUsername == Username) message.RecipientDeleted =true ;

    //if both SenderDeleted and RecipientDeleted are true then delete message
    if (message is {SenderDeleted :true,RecipientDeleted:true})
    {
      unitOfWork.MessageRepository.DeleteMessage(message);
    }
    //execute the deletion of the message
    if (await unitOfWork.Complete()) return Ok();

    return BadRequest("Problem deleting the message");
  }
}

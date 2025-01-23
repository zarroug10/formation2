using System;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Authorize]
public class MessagesController(IMessageRepository messageRepository
, IUserRepository userRepository, IMapper mapper) : BaseApiCOntroller
{

  [HttpPost]
  public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
  {
    var username = User.GetUsername();
    if (username == createMessageDTO.RecipientUsername.ToLower())
      return BadRequest("You can not send Messages to yourself");
    var sender = await userRepository.GetUserByUsernameAsync(username);
    var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

    if (sender == null || recipient == null || sender.UserName == null  ||recipient.UserName == null  )
    {
      return BadRequest("You can not send Messages");
    }
    var message = new Message
    {
      Sender = sender,
      Recipient = recipient,
      SenderUsername = sender.UserName,
      RecipientUsername = recipient.UserName,
      Content = createMessageDTO.Content
    };
    messageRepository.AddMessage(message);
    if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDTO>(message));
    return BadRequest("an error has occured connot send message");
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery]
     MessageParams messageParams)
  {
    messageParams.Username = User.GetUsername();
    var message = await messageRepository.GetMessagesforUser(messageParams);
    Response.AddPaginationHeader(message);
    return message;

  }

  [HttpGet("thread/{username}")]
  public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
  {
    var currentUsername = User.GetUsername();

    return Ok(await messageRepository.GetMessageThread(currentUsername, username) );
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteMessage(int id)
  {
    var Username = User.GetUsername();
    var message = await messageRepository.GetMessaegs(id);
    if(message == null) return BadRequest("Cannot delete this message");

    if(message.SenderUsername != Username && message.RecipientUsername != Username ) 
    return Forbid();
    if (message.SenderUsername == Username) message.SenderDeleted =true ;
    if (message.RecipientUsername == Username) message.RecipientDeleted =true ;

    if (message is {SenderDeleted :true,RecipientDeleted:true})
    {
      messageRepository.DeleteMessage(message);
    }
    if (await messageRepository.SaveAllAsync()) return Ok();

    return BadRequest("Problem deleting the message");
  }
}

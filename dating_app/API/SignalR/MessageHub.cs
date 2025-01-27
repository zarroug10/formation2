using System;

using AutoMapper;
using Microsoft.AspNetCore.SignalR;

using API.DTO;
using API.Entities;
using API.Extensions;
using API.interfaces;

namespace API.SignalR;

public class MessageHub(IUnitOfWork unitOfWork,IMapper mapper,IHubContext<PresenceHub> presencehub): Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpcontext = Context.GetHttpContext();
        var otherUser = httpcontext?.Request.Query["user"];

        if(Context.User == null || string.IsNullOrEmpty(otherUser)) throw  new Exception("Cannot join group");

        var GroupName= GetGroupName(Context.User.GetUsername(),otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId , GroupName);
        var group = await AddtoGroup(GroupName);

        await Clients.Group(GroupName).SendAsync("UpdatedGroup",group);  


        var messages = await unitOfWork.MessageRepository.GetMessageThread(Context.User.GetUsername(),otherUser!);

        if(unitOfWork.HasChanges()) await unitOfWork.Complete();

        await Clients.Caller.SendAsync("ReceiveMessageThread",messages);
    }

    public override async Task<Task> OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup",group);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDTO createMessageDTO)
    {
        var username = Context.User?.GetUsername() ?? 
        throw new HubException("Could not get User");

        if (username == createMessageDTO.RecipientUsername.ToLower())
         throw new HubException("You can not message your Self");

        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

        if (sender == null || recipient == null || sender.UserName == null  ||recipient.UserName == null  )
        {
        throw new HubException("You can not send Messages");
        }
        var message = new Message
        {
        Sender = sender,
        Recipient = recipient,
        SenderUsername = sender.UserName,
        RecipientUsername = recipient.UserName,
        Content = createMessageDTO.Content
        };

        var GroupName = GetGroupName(sender.UserName,recipient.UserName);
        var group = await unitOfWork.MessageRepository.GetMessageGroup(GroupName);

        if(group != null && group.Connections.Any(x=> x.username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        } else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if(connections != null && connections.Count != null)
            {
                await presencehub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                new {username = sender.UserName , knownAs = sender.KnownAs});
            }
        }

        unitOfWork.MessageRepository.AddMessage(message);
        if (await unitOfWork.Complete())
        {
            await Clients.Group(GroupName).SendAsync("NewMessage",mapper.Map<MessageDTO>(message));
        }
    }
    private async Task<Group> AddtoGroup(string GroupName)
    {
        var Username = Context.User?.GetUsername() ?? throw new Exception("Cannot Get Username");
        var group = await unitOfWork.MessageRepository.GetMessageGroup(GroupName);
        var Connection = new Connection{ConnectionId = Context.ConnectionId, username = Username };

        if (group ==null)
        {
            group = new Group{Name = GroupName};
            unitOfWork.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(Connection);
        if (await unitOfWork.Complete()) return group;
        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageGroup ()
    {
        var group= await unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(x=> x.ConnectionId == Context.ConnectionId);

        if(connection!=null && group!=null )
        {
            unitOfWork.MessageRepository.RemoveConnection(connection);
           if ( await unitOfWork.Complete()) return group;
        }
        
        throw new Exception("Failed to Remove From Group");
    }


    private string GetGroupName(string caller , string? other)
    {
        var stringCompare = string.CompareOrdinal(caller ,other) < 0 ;
        return stringCompare ? $"{caller}-{other}" :$"{other}-{caller}";
    }
}

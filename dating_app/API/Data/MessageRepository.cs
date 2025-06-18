using System;

using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using API.DTO;
using API.Entities;
using API.Helpers;
using API.interfaces;

namespace API.Data;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
{

    //Adding the message
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }
    //Delete message
    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<pagedList<MessageDTO>> GetMessagesforUser(MessageParams messageParams)
    {
        //Order the messages by the message sent  by descending as Querable
        var query = context.
        Messages
        .OrderByDescending(x => x.MessaegSent)
        .AsQueryable();
        //value of the query is the container in the messageparams
        query = messageParams.Container switch
        {//in case of  inbox the previous query filter it where the recipient username is equal to the messageparams's username and the recipient delated message is false
            "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username &&
                x.RecipientDeleted == false),
         //in case of the container outbox query filter where the sender's username is equal to the message prams username and the sender delted message is false        
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username &&
                x.SenderDeleted == false),
         //in case of null query filter where the recipient username is equal to the message prams username and the dateread is null and the recipient delated message is false       
            _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null &&
                x.RecipientDeleted == false)
        };
         // project the query of type message into the message dto 
        var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);
        //return the messages with pagedlist with type of message dto
        return await pagedList<MessageDTO>.CreateAsync(messages,
                    messageParams.PageNumber,
                    messageParams.PageSize);
    }

    public async Task<Message?> GetMessaegs(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var messages = context.Messages
        .Where(
            x => x.RecipientUsername == currentUsername
            && x.RecipientDeleted == false
            && x.SenderUsername == recipientUsername ||
            x.SenderUsername == currentUsername
            && x.RecipientDeleted == false
            && x.RecipientUsername == recipientUsername
        )
        .OrderBy(x => x.MessaegSent)
        .AsQueryable();

        var unreadMessages = messages.Where(x => x.DateRead == null
        && x.RecipientUsername == currentUsername).ToList();
        if (unreadMessages.Count() != 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }
        return await messages
                .ProjectTo<MessageDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
    }

    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }

    public async Task<Connection?> GetConnection(string ConnectionId)
    {
        return await context.Connections.FindAsync(ConnectionId);
    }

    public async Task<Group?> GetMessageGroup(string GroupName)
    {
        return await context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == GroupName);
    }

    public async Task<Group?> GetGroupForConnection(string ConnectionId)
    {
        return await context.Groups
        .Include(x => x.Connections)
        .Where(x => x.Connections.Any(c => c.ConnectionId == ConnectionId))
        .FirstOrDefaultAsync();
    }
}

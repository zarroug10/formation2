using System;
using API.DTO;
using API.Entities;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<pagedList<MessageDTO>> GetMessagesforUser(MessageParams messageParams)
    {
        var query = context.Messages
        .OrderByDescending(x => x.MessaegSent)
        .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username &&
                x.RecipientDeleted == false),
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username&&
                x.SenderDeleted == false),
            _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null&&
                x.RecipientDeleted == false)

        };
        var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);
        return await pagedList<MessageDTO>.CreateAsync(messages,
        messageParams.PageNumber
        , messageParams.PageSize);
    }

    public async Task<Message?> GetMessaegs(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var messages = await context.Messages
        .Where(
            x=> x.RecipientUsername == currentUsername 
            &&  x.RecipientDeleted == false   
            && x.SenderUsername == recipientUsername ||
            x.SenderUsername == currentUsername 
            &&  x.RecipientDeleted == false 
            &&  x.RecipientUsername == recipientUsername
        )
        .OrderBy(x=> x.MessaegSent)
        .ProjectTo<MessageDTO>(mapper.ConfigurationProvider)
        .ToListAsync();
        var unreadMessages = messages.Where(x=> x.DateRead == null 
        && x.RecipientUsername == currentUsername).ToList();
        if(unreadMessages.Count() != 0) 
        {
            unreadMessages.ForEach(x=> x.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        } 
        return messages;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
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
            .Include(x=> x.Connections)
            .FirstOrDefaultAsync(x=> x.Name == GroupName );
    }

    public async Task<Group?> GetGroupForConnection(string ConnectionId)
    {
        return await context.Groups
        .Include(x=> x.Connections)
        .Where(x => x.Connections.Any(c => c.ConnectionId == ConnectionId))
        .FirstOrDefaultAsync();
    }
}

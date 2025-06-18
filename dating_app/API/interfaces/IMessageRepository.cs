using System;

using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);

    void DeleteMessage (Message message);

    Task<Message?> GetMessaegs(int id);
    Task<pagedList<MessageDTO>> GetMessagesforUser(MessageParams messageParams);
    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection?> GetConnection(string ConnectionId);
    Task<Group?> GetMessageGroup(string GroupName);
    Task<Group?> GetGroupForConnection (string ConnectionId);
}

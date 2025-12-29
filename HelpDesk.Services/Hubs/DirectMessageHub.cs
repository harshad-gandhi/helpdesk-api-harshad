using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HelpDesk.Services.Hubs
{
  [Authorize]
  public class DirectMessageHub : Hub
  {
    private static readonly Dictionary<string, HashSet<string>> _connections = [];

    public override async Task OnConnectedAsync()
    {
      string? userId = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
      if (userId != null)
      {
        lock (_connections)
        {
          if (!_connections.TryGetValue(userId, out HashSet<string>? value))
          {
            value = [];
            _connections[userId] = value;
          }

          value.Add(Context.ConnectionId);
        }
      }

      await BroadcastOnlineUsers();
      await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
      string? userId = Context.UserIdentifier!;
      lock (_connections)
      {
        if (_connections.TryGetValue(userId, out HashSet<string>? value))
        {
          value.Remove(Context.ConnectionId);
          if (value.Count == 0)
            _connections.Remove(userId);
        }
      }
      await BroadcastOnlineUsers();
      await base.OnDisconnectedAsync(exception);

    }

    private Task BroadcastOnlineUsers()
    {
      List<string>? onlineUsers = [.. _connections.Keys];
      return Clients.All.SendAsync("UpdateOnlineUsers", onlineUsers);
    }

    // Start Typing indicator
    public async Task Typing(string receiverId)
    {
      string? senderId = Context.UserIdentifier!;
      if (_connections.TryGetValue(receiverId, out var receiverConnections))
      {
        foreach (var connectionId in receiverConnections)
          await Clients.Client(connectionId).SendAsync("StartedTyping", senderId);
      }
    }

    // Stop Typing indicator 
    public async Task StoppedTyping(string receiverId)
    {
      string? senderId = Context.UserIdentifier!;
      if (_connections.TryGetValue(receiverId, out var receiverConnections))
      {
        foreach (var connectionId in receiverConnections)
          await Clients.Client(connectionId).SendAsync("StoppedTyping", senderId);
      }
    }
  }
}
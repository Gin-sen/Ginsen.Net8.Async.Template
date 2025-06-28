using Microsoft.AspNetCore.SignalR;

namespace Ginsen.Net8.Async.Milestone.BlazorBackOffice.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
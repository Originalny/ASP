using Microsoft.AspNetCore.SignalR;

namespace CookieChat.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        var userName = Context.User?.Identity?.Name ?? "Аноним";
        var time = DateTime.Now.ToString("HH:mm:ss");

        await Clients.All.SendAsync("ReceiveMessage", userName, message, time);
    }
}

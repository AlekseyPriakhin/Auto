using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs;

public class OwnerHub : Hub
{
    public void Notify(string message)
    {
        Clients.All.SendAsync("GetMessage", message);
    }

    public override Task OnConnectedAsync()
    {
        var group = Context.GetHttpContext()?.Request.QueryString.Value?.Contains("group=owner");
        if (group != null)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, "owners");
        }
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnect {exception}");
        return base.OnDisconnectedAsync(exception);
    }
}
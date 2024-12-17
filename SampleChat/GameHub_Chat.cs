using Microsoft.AspNetCore.SignalR;
using SampleGame.Define;

namespace SampleChat;

public partial class GameHub
{
    public async Task SendChat(string message)
    {
        var mplayer = UserCache[Context.ConnectionId];

        var chat = new HubReceiveChat
        {
            playerid = mplayer.id,
            name = mplayer.name,
            message = message,
            time = DateTime.UtcNow,
        };

        await Clients.All.SendAsync(nameof(HubReceiveChat), chat);
    }

    public async Task JoinGroup(string key)
    {
        if (GroupDic.TryGetValue(Context.ConnectionId, out var group))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, key);
        GroupDic[Context.ConnectionId] = key;
    }

    public async Task SendGroup(string message)
    {
        if (GroupDic.TryGetValue(Context.ConnectionId, out var group))
        {
            var mplayer = UserCache[Context.ConnectionId];
            var chat = new HubReceiveChat
            {
                playerid = mplayer.id,
                name = mplayer.name,
                message = message,
                time = DateTime.UtcNow,
            };

            await Clients.Group(group).SendAsync(nameof(HubReceiveChat), chat);
        }
    }
}
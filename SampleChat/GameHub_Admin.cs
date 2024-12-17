using Microsoft.AspNetCore.SignalR;
using SampleGame.Define;

namespace SampleChat;

public partial class GameHub
{
    public async Task Call_OnPostAll()
    {
        // 일괄 메시지 전송
        await Clients.All.SendAsync(nameof(HubOnPost), new HubOnPost());
    }

    public async Task Call_OnPostPlayer(long playerid)
    {
        // 타겟 메시지 전송
        if (ConnectionidDic.TryGetValue(playerid, out var connectionid))
        {
            await Clients.Client(connectionid).SendAsync(nameof(HubOnPost), new HubOnPost());
        }
    }
}
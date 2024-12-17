using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using SampleGame.DB;

namespace SampleChat;

public partial class GameHub : Hub
{
    private static readonly ConcurrentDictionary<string, MongoPlayer> UserCache = new ConcurrentDictionary<string, MongoPlayer>();
    private static readonly ConcurrentDictionary<long, string> ConnectionidDic = new ConcurrentDictionary<long, string>();
    private static readonly ConcurrentDictionary<string, string> GroupDic = new ConcurrentDictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        var token = Context.GetHttpContext()?.Request.Query["token"];
        if (string.IsNullOrEmpty(token))
        {
            // 연결을 거부
            Context.Abort();
            return;
        }

        // 토큰 검증 및 데이터베이스 조회
        var (_, mplayer) = await UserDB.Player.GetByToken(token);
        if (null == mplayer)
        {
            // 인증 실패 시 연결 종료
            Context.Abort();
            return;
        }

        // 사용자 데이터를 메모리에 저장
        UserCache[Context.ConnectionId] = mplayer;
        ConnectionidDic[mplayer.id] = Context.ConnectionId;

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        UserCache.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }
}
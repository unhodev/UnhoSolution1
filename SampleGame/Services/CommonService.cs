using SampleGame.DB;
using SampleGame.Define;

namespace SampleGame.Services;

public static class CommonService
{
    public static async Task<(MongoResult mr, MongoPlayer mplayer)> CreatePlayer(string accesskey)
    {
        NEW_PLAYERID:
        var playerid = Random.Shared.NextInt64();
        var (mr, mplayer) = await UserDB.Player.Create(playerid, accesskey);
        if (mr == MongoResult.DUPLICATE_KEY)
            goto NEW_PLAYERID;
        return (mr, mplayer);
    }

    public static async Task<(MongoResult mr, string token)> UpdatePlayerToken(long playerid)
    {
        var expire = DateTime.UtcNow.AddMinutes(5);
        NEW_TOKEN:
        var token = Guid.NewGuid().ToString();
        var mr = await UserDB.Player.UpdateToken(playerid, token, expire);
        if (mr == MongoResult.DUPLICATE_KEY)
            goto NEW_TOKEN;
        return (mr, token);
    }

    public static async Task<(ErrorCode ec, MongoPlayer mplayer)> CheckToken(string token)
    {
        var (mr, mplayer) = await UserDB.Player.GetByToken(token);
        if (mr != MongoResult.SUCCESS)
            return (ErrorCode.DATABASE_ERROR, default);

        if (mplayer == null)
            return (ErrorCode.INVALID_TOKEN, default);

        if (mplayer.tokenexpire < DateTime.UtcNow)
            return (ErrorCode.EXPIRED_TOKEN, mplayer);

        return (ErrorCode.SUCCESS, mplayer);
    }
}
using Microsoft.AspNetCore.Mvc;
using SampleGame.DB;
using SampleGame.Define;
using SampleGame.Services;

namespace SampleGame.Controllers;

public class PlayerController : SGameControllerBase
{
    public async Task<IActionResult> Login(string key)
    {
        var (mr, mplayer) = await UserDB.Player.GetByAccessKey(key);
        if (mr != MongoResult.SUCCESS)
            return Error(mr);

        if (null == mplayer)
        {
            (mr, mplayer) = await CommonService.CreatePlayer(key);
            if (mr != MongoResult.SUCCESS)
                return Error(mr);
        }

        (mr, var token) = await CommonService.UpdatePlayerToken(mplayer.id);
        if (mr != MongoResult.SUCCESS)
            return Error(mr);

        SetCurrentPlayer(mplayer);

        return Success(new
        {
            token,
            playerid = mplayer.id,
            mplayer.name,
        });
    }

    public async Task<IActionResult> UpdateName(string token, string name)
    {
        var (ec, mplayer) = await CheckToken(token);
        if (ec != ErrorCode.SUCCESS)
            return Error(ec);

        if (string.IsNullOrEmpty(name))
            return Error(ErrorCode.INVALID_PARAMETER);
        if (name.Length <= 2)
            return Error(ErrorCode.NAME_TOO_SHORT);
        if (name.Length > 20)
            return Error(ErrorCode.NAME_TOO_LONG);

        var mr = await UserDB.Player.UpdateName(mplayer.id, name);
        if (mr != MongoResult.SUCCESS)
            return Error(mr);

        return Success(new
        {
            _name = mplayer.name,
        });
    }
}
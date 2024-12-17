using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SampleGame.DB;
using SampleGame.Define;
using SampleGame.Helpers;
using SampleGame.Services;

namespace SampleGame.Controllers;

public class SGameControllerBase : Controller
{
    private DateTime _begin;
    public ErrorCode _code = ErrorCode.FAIL;
    private string _path;
    public long _playerid;
    public object _rescontents;
    private Dictionary<string, string> _queries;
    private MongoPlayer _mplayer;
    private MongoPlayerLog _mplog;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _begin = DateTime.UtcNow;
        _path = context.HttpContext.Request.Path.ToString();
        _queries = context.HttpContext.Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());

        // REQ 로깅
        FileLogService.Write($"{HttpContext.Connection.Id} {_path} {JsonConvert.SerializeObject(_queries)}");

        // 실행
        var result = await next();
        if (result.Exception != null && result.ExceptionHandled == false)
        {
            result.ExceptionHandled = true;
            result.Result = Exception(result.Exception);
        }

        // RES 로깅
        var ms = (int)(DateTime.UtcNow - _begin).TotalMilliseconds;
        var contents = _rescontents;
        FileLogService.Write($"{HttpContext.Connection.Id} {_playerid} {_code} {ms}ms {JsonConvert.SerializeObject(contents)}");

        _ = Task.Run(async () =>
        {
            if (_playerid == 0)
                return;
            if (_mplog == null)
                await LogDB.PlayerLog.Add(_begin, _playerid, _path, _queries, _rescontents, _code, ms);
            else
                await LogDB.PlayerLog.Update(_begin, _mplog.id, _rescontents, _code, ms);
        });
    }

    protected IActionResult Success(object data)
    {
        var obj = JObject.FromObject(data);
        obj[nameof(ResBase.code)] = (int)ErrorCode.SUCCESS;
        _code = ErrorCode.SUCCESS;
        _rescontents = data;
        return Content(JsonConvert.SerializeObject(obj), "application/json");
    }

    protected IActionResult Error(ErrorCode code = ErrorCode.FAIL, object msg = default, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
    {
        _code = code;
        _rescontents = new
        {
            code = (int)code,
            debug = $"{code} {msg} {Path.GetFileName(file)}:{line}",
        };

        return Content(JsonConvert.SerializeObject(_rescontents), "application/json");
    }

    protected IActionResult Error(MongoResult code, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
    {
        _code = ErrorCode.DATABASE_ERROR;
        _rescontents = new
        {
            code = (int)code,
            debug = $"{code} {Path.GetFileName(file)}:{line}",
        };

        return Content(JsonConvert.SerializeObject(_rescontents), "application/json");
    }

    private IActionResult Exception(Exception ex)
    {
        var stackTrace = new StackTrace(ex, true); // true: 디버그 정보 포함

        var file = string.Empty;
        var line = -1;

        if (stackTrace.GetFrame(0) is { } frame)
        {
            file = Path.GetFileName(frame.GetFileName());
            line = frame.GetFileLineNumber();
        }

        _code = ErrorCode.EXCEPTION;
        _rescontents = new
        {
            code = (int)_code,
            debug = $"{_code} {ex.Message} {file}:{line}",
        };

        return Content(JsonConvert.SerializeObject(_rescontents), "application/json");
    }

    protected void SetCurrentPlayer(MongoPlayer mplayer)
    {
        _playerid = mplayer.id;
        _mplayer = mplayer;
    }

    protected async Task<(ErrorCode ec, MongoPlayer mplayer)> CheckToken(string token)
    {
        var (ec, mplayer) = await CommonService.CheckToken(token);
        if (mplayer != null)
        {
            SetCurrentPlayer(mplayer);
            (_, _mplog) = await LogDB.PlayerLog.Add(_begin, _playerid, _path, _queries);
        }

        return (ec, mplayer);
    }
}
using Microsoft.AspNetCore.Mvc;
using SampleGame.DB;

namespace SampleAdmin.Controllers;

public class PlayerLogController : ControllerBase
{
    public async Task<IActionResult> List()
    {
        var (_, list) = await LogDB.PlayerLog.Tail();
        return new JsonResult(list);
    }
}
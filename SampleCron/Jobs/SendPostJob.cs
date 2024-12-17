using SampleGame.DB;
using SampleGame.Helpers;

namespace SampleCron.Jobs;

public class SendPostJob
{
    public static async Task ExecuteAsync(DateTime time)
    {
        var (_, mjobs) = await AdminDB.SendPostJob.GetList_Ready(time);
        if (mjobs.Count == 0)
            return;

        foreach (var mjob in mjobs)
        {
            var (_, ok) = await AdminDB.SendPostJob.SetState_ReadyToRunning(mjob.id);
            if (!ok)
                continue;

            FileLogService.Write($"{nameof(SendPostJob)} {mjob.id} {time}");
            var begin = DateTime.UtcNow;
            var r = await UserDB.Player.ForEachAll(async x =>
            {
                var mr = await UserDB.Post.Add(x.id, time, mjob.endtime, mjob.rewards, mjob.id);
                if (mr == MongoResult.SUCCESS)
                    return true;

                FileLogService.Write($"{nameof(SendPostJob)} {mjob.id} {x.id} {mr}");
                return false;
            });

            FileLogService.Write($"{nameof(SendPostJob)} {mjob.id} {(DateTime.UtcNow - begin).TotalMicroseconds}ms total={r.total},ok={r.ok}");
            await AdminDB.SendPostJob.SetState_RunningToDone(mjob.id);
        }
    }
}
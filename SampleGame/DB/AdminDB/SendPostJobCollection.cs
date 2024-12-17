using MongoDB.Driver;
using SampleGame.Define;

namespace SampleGame.DB;

public static partial class AdminDB
{
    public static SendPostJobCollection SendPostJob { get; } = new SendPostJobCollection();

    public class SendPostJobCollection : CollectionBase
    {
        private IMongoCollection<MongoSendPostJob> _col;

        public override void Init(IMongoDatabase db, string colname)
        {
            base.Init(db, colname);
            _col = db.GetCollection<MongoSendPostJob>(colname);
        }

        public async Task<(MongoResult mr, List<MongoSendPostJob> mjobs)> GetList_Ready(DateTime time)
        {
            try
            {
                var filter = Builders<MongoSendPostJob>.Filter.And(
                    Builders<MongoSendPostJob>.Filter.Eq(x => x.jobstate, JobState.READY),
                    Builders<MongoSendPostJob>.Filter.Lte(x => x.starttime, time)
                );
                var list = await _col.Find(filter).ToListAsync();
                return (MongoResult.SUCCESS, list);
            }
            catch (Exception e)
            {
                return (e2r(e), default);
            }
        }

        public async Task<(MongoResult mr, bool ok)> SetState_ReadyToRunning(int docid)
        {
            try
            {
                var filter = Builders<MongoSendPostJob>.Filter.And(
                    Builders<MongoSendPostJob>.Filter.Eq(x => x.id, docid),
                    Builders<MongoSendPostJob>.Filter.Eq(x => x.jobstate, JobState.READY)
                );
                var update = Builders<MongoSendPostJob>.Update.Set(x => x.jobstate, JobState.RUNNING);
                var r = await _col.UpdateOneAsync(filter, update);
                return (MongoResult.SUCCESS, r.ModifiedCount == 1);
            }
            catch (Exception e)
            {
                return (e2r(e), default);
            }
        }

        public async Task<(MongoResult mr, bool ok)> SetState_RunningToDone(int docid)
        {
            try
            {
                var filter = Builders<MongoSendPostJob>.Filter.And(
                    Builders<MongoSendPostJob>.Filter.Eq(x => x.id, docid),
                    Builders<MongoSendPostJob>.Filter.Eq(x => x.jobstate, JobState.RUNNING)
                );
                var update = Builders<MongoSendPostJob>.Update.Set(x => x.jobstate, JobState.DONE);
                var r = await _col.UpdateOneAsync(filter, update);
                return (MongoResult.SUCCESS, r.ModifiedCount == 1);
            }
            catch (Exception e)
            {
                return (e2r(e), default);
            }
        }
    }
}
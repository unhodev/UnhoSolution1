using MongoDB.Driver;
using SampleGame.Define;

namespace SampleGame.DB;

public static partial class UserDB
{
    public static PostCollection Post { get; } = new PostCollection();

    public class PostCollection : PlayerNCollectionBase
    {
        private IMongoCollection<MongoPost> _col;

        public override void Init(IMongoDatabase db, string colname)
        {
            base.Init(db, colname);
            _col = db.GetCollection<MongoPost>(colname);
            _col.Indexes.CreateOne(new CreateIndexModel<MongoPost>(Builders<MongoPost>.IndexKeys.Ascending(x => x.playerid).Descending(x => x.create)));
        }

        public async Task<MongoResult> Add(long playerid, DateTime create, DateTime expire, List<ShareReward> rewards, int refid)
        {
            var mdoc = new MongoPost
            {
                create = create,
                expire = expire,
                playerid = playerid,
                rewards = rewards,
                refid = refid,
            };
            try
            {
                await _col.InsertOneAsync(mdoc);
                return (MongoResult.SUCCESS);
            }
            catch (Exception e)
            {
                return (e2r(e));
            }
        }
    }
}
using MongoDB.Driver;

namespace SampleGame.DB;

public static partial class UserDB
{
    public static PlayerCollection Player { get; } = new PlayerCollection();

    public class PlayerCollection : Player1CollectionBase
    {
        private IMongoCollection<MongoPlayer> _col;

        public override void Init(IMongoDatabase db, string colname)
        {
            base.Init(db, colname);

            var col = db.GetCollection<MongoPlayer>(colname);
            col.Indexes.CreateOne(new CreateIndexModel<MongoPlayer>(
                Builders<MongoPlayer>.IndexKeys.Ascending(x => x.token),
                new CreateIndexOptions<MongoPlayer>
                {
                    PartialFilterExpression = Builders<MongoPlayer>.Filter.Gt(x => x.token, string.Empty), // NOTE: null 허용
                    Unique = true,
                })
            );
            col.Indexes.CreateOne(new CreateIndexModel<MongoPlayer>(
                Builders<MongoPlayer>.IndexKeys.Ascending(x => x.accesskey),
                new CreateIndexOptions<MongoPlayer>
                {
                    PartialFilterExpression = Builders<MongoPlayer>.Filter.Gt(x => x.token, string.Empty), // NOTE: null 허용
                    Unique = true,
                })
            );

            _col = col;
        }


        public async Task<(MongoResult mr, MongoPlayer mplayer)> GetByAccessKey(string accesskey)
        {
            try
            {
                var doc = await _col.Find(x => x.accesskey == accesskey).FirstOrDefaultAsync();
                return (MongoResult.SUCCESS, doc);
            }
            catch (Exception ex)
            {
                return (e2r(ex), default);
            }
        }

        public async Task<(MongoResult mr, MongoPlayer mplayer)> GetByToken(string token)
        {
            try
            {
                var doc = await _col.Find(x => x.token == token).FirstOrDefaultAsync();
                return (MongoResult.SUCCESS, doc);
            }
            catch (Exception ex)
            {
                return (e2r(ex), default);
            }
        }

        public async Task<(MongoResult mr, MongoPlayer mplayer)> Get(long playerid)
        {
            try
            {
                var doc = await _col.Find(x => x.id == playerid).FirstOrDefaultAsync();
                return (MongoResult.SUCCESS, doc);
            }
            catch (Exception ex)
            {
                return (e2r(ex), default);
            }
        }

        public async Task<(MongoResult SUCCESS, MongoPlayer mplayer)> Create(long playerid, string accesskey)
        {
            try
            {
                var doc = new MongoPlayer()
                {
                    id = playerid,
                    accesskey = accesskey,
                    token = string.Empty,
                    tokenexpire = DateTime.MinValue,
                };

                await _col.InsertOneAsync(doc);
                return (MongoResult.SUCCESS, doc);
            }
            catch (Exception ex)
            {
                return (e2r(ex), default);
            }
        }

        public async Task<MongoResult> UpdateToken(long playerid, string token, DateTime expire)
        {
            try
            {
                var filter = Builders<MongoPlayer>.Filter.Eq(x => x.id, playerid);
                var update = Builders<MongoPlayer>.Update
                        .Set(x => x.token, token)
                        .Set(x => x.tokenexpire, expire)
                    ;
                _ = await _col.UpdateOneAsync(filter, update);
                return (MongoResult.SUCCESS);
            }
            catch (Exception ex)
            {
                return (e2r(ex));
            }
        }

        public async Task<MongoResult> UpdateName(long playerid, string name)
        {
            try
            {
                var filter = Builders<MongoPlayer>.Filter.Eq(x => x.id, playerid);
                var update = Builders<MongoPlayer>.Update.Set(x => x.name, name);
                _ = await _col.UpdateOneAsync(filter, update);
                return (MongoResult.SUCCESS);
            }
            catch (Exception ex)
            {
                return (e2r(ex));
            }
        }

        public async Task<(int total, int ok)> ForEachAll(Func<MongoPlayer, Task<bool>> func1)
        {
            var filter = Builders<MongoPlayer>.Filter.Empty;
            var options = new FindOptions { BatchSize = 1000 };
            var tasks = new List<Task<bool>>();

            await _col.Find(filter, options).ForEachAsync(x => tasks.Add(func1(x)));

            var r = await Task.WhenAll(tasks);

            return (tasks.Count, r.Count(x => x));
        }
    }
}
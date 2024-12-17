using MongoDB.Bson;
using MongoDB.Driver;
using SampleGame.Define;

namespace SampleGame.DB;

public static partial class LogDB
{
    public static PlayerLogCollection PlayerLog = new PlayerLogCollection();


    // TODO : PartitionCollectionBase
    public class PlayerLogCollection : CollectionBase
    {
        private IMongoCollection<MongoPlayerLog> _col;

        public override void Init(IMongoDatabase db, string colname)
        {
            var col = db.GetCollection<MongoPlayerLog>(colname);
            col.Indexes.CreateOne(new CreateIndexModel<MongoPlayerLog>(Builders<MongoPlayerLog>.IndexKeys.Descending(x => x.time)));
            col.Indexes.CreateOne(new CreateIndexModel<MongoPlayerLog>(Builders<MongoPlayerLog>.IndexKeys.Ascending(x => x.playerid).Descending(x => x.time)));
            _col = col;
        }

        protected IMongoCollection<MongoPlayerLog> GetCol(DateTime time) => _col;

        public async Task<(MongoResult mr, MongoPlayerLog mplog)> Add(DateTime time, long playerid, string path, Dictionary<string, string> request)
        {
            var mplog = new MongoPlayerLog()
            {
                time = time,
                playerid = playerid,
                path = path,
                request = request,
                code = (int)ErrorCode.FAIL,
                ms = int.MaxValue,
            };

            try
            {
                await GetCol(time).InsertOneAsync(mplog);
                return (MongoResult.SUCCESS, mplog);
            }
            catch (Exception e)
            {
                return (e2r(e), default);
            }
        }

        public async Task<MongoResult> Add(DateTime time, long playerid, string path, Dictionary<string, string> request, object response, ErrorCode code, int ms)
        {
            var mplog = new MongoPlayerLog()
            {
                time = time,
                playerid = playerid,
                path = path,
                request = request,
                response = response,
                ms = ms,
                code = (int)code,
            };

            try
            {
                await GetCol(time).InsertOneAsync(mplog);
                return (MongoResult.SUCCESS);
            }
            catch (Exception e)
            {
                return (e2r(e));
            }
        }

        public async Task<MongoResult> Update(DateTime time, ObjectId docid, object response, ErrorCode code, int ms)
        {
            try
            {
                var filter = Builders<MongoPlayerLog>.Filter.Eq(x => x.id, docid);
                var update = Builders<MongoPlayerLog>.Update
                        .Set(x => x.response, response)
                        .Set(x => x.code, (int)code)
                        .Set(x => x.ms, ms)
                    ;

                _ = await GetCol(time).UpdateOneAsync(filter, update);
                return (MongoResult.SUCCESS);
            }
            catch (Exception e)
            {
                return (e2r(e));
            }
        }

        public async Task<(MongoResult mr, List<MongoPlayerLog> mplogs)> Tail()
        {
            try
            {
                var filter = Builders<MongoPlayerLog>.Filter.Empty;
                var list = await GetCol(DateTime.Now).Find(filter).SortBy(x => x.time).Limit(10).ToListAsync();
                return (MongoResult.SUCCESS, list);
            }
            catch (Exception e)
            {
                return (e2r(e), []);
            }
        }
    }
}
using MongoDB.Driver;

namespace SampleGame.DB;

public static partial class UserDB
{
    public static void Init(string connectionString)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(nameof(UserDB));

        Player.Init(database, nameof(Player));
    }

    public abstract class Player1CollectionBase : CollectionBase
    {
        public override void Init(IMongoDatabase db, string colname)
        {
            base.Init(db, colname);
        }
    }

    public abstract class PlayerNCollectionBase : CollectionBase
    {
    }

    public static MongoResult e2r(Exception ex) => ex switch
    {
        MongoWriteException w => w.WriteError.Category == ServerErrorCategory.DuplicateKey ? MongoResult.DUPLICATE_KEY : MongoResult.FAIL,
        _ => MongoResult.FAIL,
    };
}
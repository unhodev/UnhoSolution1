using MongoDB.Driver;

namespace SampleGame.DB;

public static partial class AdminDB
{
    public static void Init(string connectionString)
    {
        var client = new MongoClient(connectionString);
        var db = client.GetDatabase(nameof(AdminDB));

        SendPostJob.Init(db, nameof(SendPostJob));
    }

    public static MongoResult e2r(Exception ex) => ex switch
    {
        MongoWriteException w => w.WriteError.Category == ServerErrorCategory.DuplicateKey ? MongoResult.DUPLICATE_KEY : MongoResult.FAIL,
        _ => MongoResult.FAIL,
    };
}
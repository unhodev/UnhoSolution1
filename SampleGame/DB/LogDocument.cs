using MongoDB.Bson;

namespace SampleGame.DB;

public class MongoPlayerLog
{
    public ObjectId id { get; set; }
    public DateTime time { get; set; }
    public string path { get; set; }
    public long playerid { get; set; }
    public Dictionary<string, string> request { get; set; }
    public object response { get; set; }
    public int ms { get; set; }
    public int code { get; set; }
}
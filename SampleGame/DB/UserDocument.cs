using MongoDB.Bson;
using SampleGame.Define;

namespace SampleGame.DB;

interface Player1Document
{
    long id { get; set; }
}

interface PlayerNDocument
{
    ObjectId id { get; set; }
    long playerid { get; set; }
}

public class MongoPlayer : Player1Document
{
    public long id { get; set; }
    public string accesskey { get; set; }
    public string token { get; set; }
    public DateTime tokenexpire { get; set; }
    public string name { get; set; }
}

public class MongoPost : PlayerNDocument
{
    public ObjectId id { get; set; }
    public long playerid { get; set; }
    public List<ShareReward> rewards { get; set; }
    public DateTime create { get; set; }
    public DateTime expire { get; set; }
    public int refid { get; set; }
}
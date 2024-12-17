using SampleGame.Define;

namespace SampleGame.DB;

public abstract class JobDocumentBase
{
    public DateTime starttime { get; set; }
    public DateTime endtime { get; set; }
    public JobState jobstate { get; set; }
}

public class MongoSendPostJob : JobDocumentBase
{
    public int id { get; set; }
    public List<ShareReward> rewards { get; set; }
}
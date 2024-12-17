using MongoDB.Driver;

namespace SampleGame.DB;

public abstract class CollectionBase
{
    public virtual void Init(IMongoDatabase db, string colname)
    {
    }
}
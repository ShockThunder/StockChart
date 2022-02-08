using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockApi;

public class StockEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string DateTime { get; set; }
    public double Value { get; set; }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactsAPI.Model.Abbreviated;

// Abbreviated form of Contact entity,
// for use when you want to extract
// a partial overview of the document
public record ContactAbbreviated()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string Name { get; set; }
    public string Surname { get; set; }
};
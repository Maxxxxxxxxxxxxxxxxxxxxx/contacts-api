using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactsAPI.Model;

public class Credentials
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Username;
    public string PasswordHash;
}
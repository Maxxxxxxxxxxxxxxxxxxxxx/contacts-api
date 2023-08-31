using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactsAPI.Model;

// MongoDB Document object
public class Contact
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
}

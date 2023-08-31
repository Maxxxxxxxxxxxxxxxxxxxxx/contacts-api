using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactsAPI.Model;

// Nested dictionary type aliases for category/subcategory handling
// Direct Contact document references are saved as stringed ObjectIds
using Subcategory = Dictionary<string, List<string>>;
using ContactsDictionary = Dictionary<Category, Dictionary<string, List<string>>>;

// UserData object. Username is unique.
public class UserData
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? Id { get; set; }
    
    [BsonRepresentation(BsonType.Document)] 
    public ContactsDictionary Contacts { get; set; }
    
    public string Username { get; set; }
    
    public UserData(string username)
    {
        Username = username;
        Contacts = new ContactsDictionary
        {
            { Category.Business, new Subcategory() },
            { Category.Private, new Subcategory() },
            { Category.Other, new Subcategory() }
        };
    }

    // Helper method for aggregating all Contact ObjectIds into a single list.
    public List<string> GetContactIds()
    {
        var userIds = new List<string>();
        foreach(var subcategory in Contacts.Values)
        {
            foreach (var ids in subcategory.Values)
            {
                userIds.AddRange(ids);
            }
        }
        return userIds.Distinct().ToList();
    }
}
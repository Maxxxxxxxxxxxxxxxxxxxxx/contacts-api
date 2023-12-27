using System.Linq.Expressions;
using ContactsAPI.Config;
using ContactsAPI.Model;
using ContactsAPI.Model.Abbreviated;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace ContactsAPI.Services;

// Service for handling Contact entities
public class CollectionsService
{
    private readonly IMongoCollection<Contact> _contactsCollection;
    private readonly MongoClient _client;

    private readonly ILogger _logger;

    public CollectionsService(IOptions<MongoSettings> dbSettings, ILogger<CollectionsService> logger)
    {
        var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        
        _contactsCollection = mongoDb.GetCollection<Contact>(dbSettings.Value.ContactCollection);

        _client = mongoClient;
        _logger = logger;
        
        _logger.LogInformation($"Connected to MongoDB at {dbSettings.Value.ConnectionString}");
    }
    
    // *****************
    // Contact methods 
    // *****************

    public async Task<List<ContactDTO>> GetAbbreviatedContactList()
    {
        var findOptions = new FindOptions<Contact>
        {
            Projection = Builders<Contact>.Projection
                .Include("_id")
                .Include("Name")
                .Include("Surname")
        };

        var cursor = await _contactsCollection.FindAsync(x => true, findOptions);
        var list = await cursor.ToListAsync();

        var values = list.Select(contact => new ContactDTO()
        {
            Id = contact.Id,
            Name = contact.Name,
            Surname = contact.Surname
        }).ToList();

        return values;
    }
    
    public async Task<List<Contact>> GetContactAsync() => 
        await _contactsCollection.Find(_ => true).ToListAsync();
    
    public async Task<Contact> GetContactAsync(string id) => 
        await _contactsCollection.Find( x => x.Id == id).SingleAsync();
    
    public async Task<List<Contact>> GetContactsFilter(Expression<Func<Contact, bool>> filter) => 
        await _contactsCollection.Find(filter).ToListAsync();

    // Saves just the contact to db.contacts and returns ObjectId
    public async Task<string> SaveContactAsync(Contact contact)
    {
        await _contactsCollection.InsertOneAsync(contact);
        return contact.Id!;
    }
    
    public async Task UpdateContactAsync(string id, Contact contact) => 
        await _contactsCollection.ReplaceOneAsync(x => x.Id == id, contact);
    
    public async Task DeleteContactAsync(string id) => 
        await _contactsCollection.DeleteOneAsync(x => x.Id == id);
}
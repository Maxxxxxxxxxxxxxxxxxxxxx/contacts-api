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
    private readonly IMongoCollection<UserData> _userDataCollection;
    private readonly IMongoCollection<Credentials> _credentialsCollection;
    private readonly MongoClient _client;

    public CollectionsService(IOptions<MongoSettings> dbSettings)
    {
        var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
        var mongoDb = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        
        _contactsCollection = mongoDb.GetCollection<Contact>(dbSettings.Value.ContactCollection);
        _userDataCollection = mongoDb.GetCollection<UserData>(dbSettings.Value.AccountCollection);
        _credentialsCollection = mongoDb.GetCollection<Credentials>(dbSettings.Value.CredentialsCollection);

        _client = mongoClient;
    }
    
    // *****************
    // Contact methods 
    // *****************

    public async Task<List<ContactAbbreviated>> GetAbbreviatedContactList()
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

        var values = list.Select(contact => new ContactAbbreviated()
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
    private async Task<string> SaveContactAsync(Contact contact)
    {
        await _contactsCollection.InsertOneAsync(contact);
        return contact.Id!;
    }

    /// <summary>
    /// Saves contact to database ensuring that bound UserData document also gets updated
    /// </summary>
    public async Task CreateContactAsync(
        Contact contact, 
        string username,
        string subcategory,
        string category)
    {
        // todo: change to transaction after I figure out why they don't work
        var objId = await SaveContactAsync(contact); // Save contact to directly to db.contacts
        await SaveContactUser(username, category, subcategory, objId); // Save contact to appropriate document in db.users
    }
    
    public async Task UpdateContactAsync(string id, Contact contact) => 
        await _contactsCollection.ReplaceOneAsync(x => x.Id == id, contact);
    
    public async Task DeleteContactAsync(string id) => 
        await _contactsCollection.DeleteOneAsync(x => x.Id == id);
    
    // *****************
    // UserData methods 
    // *****************
    
    // fetches all UserData objects
    public async Task<List<UserData>> GetUserData() => 
        await _userDataCollection.Find(_ => true).ToListAsync();
    
    // fetch single UserData by username
    public async Task<List<UserData>> GetUserData(string username) => 
        await _userDataCollection.Find(x => x.Username == username).ToListAsync();
    
    // Update UserData document in database
    public async Task UpdateUserData(string username, UserData data) => 
        await _userDataCollection.ReplaceOneAsync(x => x.Username == username, data);
    
    // Checks if UserData with specified username already exists
    public async Task<bool> UserDataExists(string username)
    {
        var cursor = await _userDataCollection.FindAsync(x => x.Username == username);
        return await cursor.AnyAsync();
    }

    // User deletion handler. Deletes UserData along with all linked Contacts, by username.
    public async Task DeepDeleteUserData(string username)
    {
        var userData = await GetUserData(username);
        var contactIds = userData.First().GetContactIds();

        await _contactsCollection.DeleteManyAsync(x => contactIds.Contains(x.Id!));
        await _userDataCollection.DeleteOneAsync(x => x.Username == username);
    }

    // Creates a new empty UserData for specified username
    // Throws an exception if UserData with the username already exists in the DB
    public async Task CreateNewUserData(string username)
    {
        if (await UserDataExists(username)) throw new Exception("User already exists!");
        
        var newUserData = new UserData(username);
        _userDataCollection.InsertOne(newUserData);
    }
    
    // Saves Contact's ObjectId to specified category/subcategory of UserData
    private async Task SaveContactUser(
        string username, 
        string category,
        string subcategory,
        string objectId)
    {
        var result = await GetUserData(username);
        var userData = result.First();
        var dictionary = userData.Contacts;

        // create subcategory if it doesn't already exist
        if (!dictionary[category].ContainsKey(subcategory)) 
            dictionary[category].Add(subcategory, new List<string>());
        
        dictionary[category][subcategory].Add(objectId);

        userData.Contacts = dictionary;

        await UpdateUserData(username, userData);
    }
    
    // *****************
    // Credentials methods 
    // *****************
    
    // Checks if UserData with specified username already exists
    public async Task<bool> CredentialsExist(string username)
    {
        var cursor = await _credentialsCollection.FindAsync(x => x.Username == username);
        return await cursor.AnyAsync();
    }

    // Saves credentials with check if user already exists
    public async Task SaveCredentialsAsync(Credentials credentials)
    {
        if(!await CredentialsExist(credentials.Username)) await _credentialsCollection.InsertOneAsync(credentials);
        
        throw new Exception("User credentials already exist!");
    }
    
    // Get user credentials document
    public async Task<List<Credentials>> GetUserCredentialsAsync(string username) =>
        await _credentialsCollection.Find(x => x.Username == username).ToListAsync();
    
    // Deletes credentials document
    public async Task DeleteCredentialsAsync(string username) =>
        await _credentialsCollection.DeleteOneAsync(x => x.Username == username);
}
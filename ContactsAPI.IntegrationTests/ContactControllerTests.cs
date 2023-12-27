using System.Net.Http.Json;
using ContactsAPI.Bodies;
using ContactsAPI.Model;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ContactsAPI.IntegrationTests;

public class ContactControllerTests
{
    // private readonly HttpClient _client;

    [Fact]
    public async Task Contact_AddContact_Register()
    {
        var app = new ContactsApiApplicationFactory();

        AddContactRequest req = new("TestUser", "Surname", "email@e.com", "P@ssw0rd", "777000111", "2001-01-01");
        
        var client = app.CreateClient();
        var response = await client.PostAsJsonAsync($"/api/contact/", req);
        
        response.EnsureSuccessStatusCode();
    
        var contact = await response.Content.ReadFromJsonAsync<Contact>();
    
        Assert.NotNull(contact);
        Assert.Equal(contact.Name, req.Name);
    }
    
    [Fact]
    public async Task Contact_TestGet()
    {
        var app = new ContactsApiApplicationFactory();
        var client = app.CreateClient();
        var response = await client.GetAsync("/api/contact/");
        response.EnsureSuccessStatusCode();
    }
}
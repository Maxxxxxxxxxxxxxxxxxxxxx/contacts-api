using System.Net.Http.Json;
using ContactsAPI.Bodies;
using ContactsAPI.Model;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ContactsAPI.IntegrationTests;

public class ContactControllerTests
{
    // private readonly HttpClient _client;

    // [Fact]
    // public async Task Contact_AddContact_Register()
    // {
    //     var app = new ContactsApiApplicationFactory();
    //     var username = "TestUser";
    //     var userPassword = "P@ssw0rd";
    //
    //     AuthRequest register = new AuthRequest(username, userPassword);
    //     AddContactRequest req = new("Imie", "Nazwisko", "imie.nazwisko@email.com", "P@ssw0rd", "666921420",
    //         "2001-04-01");
    //     
    //     var client = app.CreateClient();
    //     var response = await client.PostAsJsonAsync($"/api/contact/{username}", req);
    //     response.EnsureSuccessStatusCode();
    //
    //     var contact = await response.Content.ReadFromJsonAsync<Contact>();
    //
    //     Assert.NotNull(contact);
    // }
    
    [Fact]
    public async Task Contact_TestGet()
    {
        var app = new ContactsApiApplicationFactory();
        var client = app.CreateClient();
        var response = await client.GetAsync("/api/contact/");
        response.EnsureSuccessStatusCode();
    }
}
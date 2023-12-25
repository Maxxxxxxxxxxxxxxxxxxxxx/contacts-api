using ContactsAPI.Bodies;
using ContactsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : Controller
{
    private readonly CollectionsService _collectionsService;

    public ContactController(CollectionsService svc) => _collectionsService = svc;

    // gets Contact by ID
    [HttpGet("{id:length(24)}")]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        var contactObject = await _collectionsService.GetContactAsync(id);
        return Ok(contactObject);
    }

    // gets all Contacts
    // returns only truncated contacts data, (Id, Name, Surname)
    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var response = await _collectionsService.GetAbbreviatedContactList();
        return Ok(response);
    }

    // gets all Contacts of specified User
    [HttpGet("user/{username}")]
    public async Task<IActionResult> GetUsersContacts([FromRoute]string username)
    {
        var list = await _collectionsService.GetUserData(username);
        var ids = list.First().GetContactIds();

        var response = await _collectionsService.GetContactsFilter(x => ids.Contains(x.Id));
        
        return Ok(response);
    }
    
    // ***********************
    // Auth-required endpoints
    // ***********************

    // Update Contact
    [Authorize]
    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> UpdateContact(
        [FromRoute] string id,
        [FromBody] UpdateContactRequest request)
    {
        var contactToUpdate = await _collectionsService.GetContactAsync(id);
        var updatedContact = request.Apply(contactToUpdate);

        try
        {
            await _collectionsService.UpdateContactAsync(id, updatedContact);
            return Ok();
        }
        catch (Exception _)
        {
            return StatusCode(422);
        }
    }
    
    // Add new contact, category/subcategory specified with query parameters
    [Authorize]
    [HttpPost("user/{username}")]
    public async Task<IActionResult> NewContact(
        [FromRoute] string username,
        [FromQuery] string category,
        [FromQuery] string subcategory,
        [FromBody] AddContactRequest request)
    {
        var newContact = request.MapToContact();
        await _collectionsService.CreateContactAsync(newContact, username, subcategory, category);
        
        return Ok();
    }
}
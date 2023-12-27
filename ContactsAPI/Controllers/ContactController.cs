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
        try
        {
            var contactObject = await _collectionsService.GetContactAsync(id);
            return Ok(contactObject);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    // gets all Contacts
    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        try
        {
            var response = await _collectionsService.GetContactsFilter(c => true);
            return Ok(response);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
    
    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> UpdateContact(
        [FromRoute] string id,
        [FromBody] UpdateContactRequest request)
    {
        var contactToUpdate = await _collectionsService.GetContactAsync(id);
        var updatedContact = request.Apply(contactToUpdate);

        await _collectionsService.UpdateContactAsync(id, updatedContact);
        return Ok(updatedContact);
    }
    
    [HttpPost("")]
    public async Task<IActionResult> NewContact(
        [FromBody] AddContactRequest request)
    {
        var newContact = request.MapToContact();
        await _collectionsService.SaveContactAsync(newContact);
        return Ok(newContact);
    }
}
using ContactsAPI.Model;

namespace ContactsAPI.Bodies;

public record AddContactRequest
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string DateOfBirth { get; set; }

    public Contact MapToContact()
    {
        var entity = new Contact
        {
          Name = Name,
          Surname = Surname,
          Email = Email,
          Password = Password,
          PhoneNumber = PhoneNumber,
          DateOfBirth = DateOfBirth // todo: map date string to date object
        };
        
        return entity;
    }
}
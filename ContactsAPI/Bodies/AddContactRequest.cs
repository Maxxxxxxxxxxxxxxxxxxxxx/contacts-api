using ContactsAPI.Model;

namespace ContactsAPI.Bodies;

public record AddContactRequest
{
    private string Name { get; set; }
    private string Surname { get; set; }
    private string Email { get; set; }
    private string Password { get; set; }
    private string PhoneNumber { get; set; }
    private string DateOfBirth { get; set; }

    public AddContactRequest(string name, string surname, string email, string password, string phoneNumber, string dateOfBirth)
    {
        Name = name;
        Surname = surname;
        Email = email;
        Password = password;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
    }

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
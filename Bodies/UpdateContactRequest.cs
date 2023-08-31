using ContactsAPI.Model;

namespace ContactsAPI.Bodies;

public record UpdateContactRequest
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DateOfBirth { get; set; }

    public Contact Apply(Contact destination)
    {
        if (Name        != null) destination.Name = Name;
        if (Surname     != null) destination.Surname = Surname;
        if (Email       != null) destination.Email = Email;
        if (Password    != null) destination.Password = Password;
        if (PhoneNumber != null) destination.PhoneNumber = PhoneNumber;
        if (DateOfBirth != null) destination.DateOfBirth = DateOfBirth;

        return destination;
    }
}
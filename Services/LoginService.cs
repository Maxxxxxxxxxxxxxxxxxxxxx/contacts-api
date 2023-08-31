using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PasswordCheckerLibrary;
using Credentials = ContactsAPI.Model.Credentials;

namespace ContactsAPI.Services;

// Service for handling user data and login, JWT generation
public class LoginService
{
    private readonly IConfiguration _config;
    private static TimeSpan _tokenLifetime;
    private readonly PasswordSecurityChecker _passwordChecker = new PasswordSecurityChecker();
    private readonly CollectionsService _collectionsService;

    public LoginService(IConfiguration config, CollectionsService svc)
    {
        var minutes = config
            .GetSection("JWTSettings")
            .GetValue<int>("TokenLifetime");
        
        _config = config;
        _collectionsService = svc;
        _tokenLifetime = TimeSpan.FromMinutes(minutes);
    }

    public bool IsPasswordSecure(string password) => _passwordChecker.IsPasswordSecure(password);

    public bool IsEmailValid(string email) => EmailValidation.EmailValidator.Validate(email);

    public string HashPassword(string password) => PasswordHash.Encrypt.SHA512(password);

    // Generates JWT
    private string GenerateToken(Credentials credentials)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSettings:Secret"]!));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,credentials.Username),
        };
        
        var token = new JwtSecurityToken(_config["JWTSettings:Issuer"],
            _config["JWTSettings:Audience"],
            claims,
            expires: DateTime.Now.Add(_tokenLifetime),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task RegisterUserAsync(Credentials credentials)
    {
        try
        {
            await _collectionsService.CreateNewUserData(credentials.Username); // Create new UserData entity
            await _collectionsService.SaveCredentialsAsync(credentials); // Save credentials
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
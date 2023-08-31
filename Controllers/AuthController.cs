using ContactsAPI.Bodies;
using ContactsAPI.Model;
using ContactsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly LoginService _loginService;

    public AuthController(LoginService svc) => _loginService = svc;
    
    [HttpGet("login")]
    public async Task<IActionResult> Login([FromBody]AuthRequest request)
    {
        if (!await _loginService.ValidateLoginCredentials(request.Username, request.Password))
        {
            return StatusCode(401); // returns code 401 if login failed for any reason
        }
        
        var credentials = new Credentials
        {
            Username = request.Username,
            PasswordHash = _loginService.HashPassword(request.Password)
        };
            
        var token = _loginService.GenerateToken(credentials);
        return Ok(token); // returns JWT if login succeded
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]AuthRequest request)
    {
        if (_loginService.IsPasswordSecure(request.Password) 
            && _loginService.IsUsernameValid(request.Username))
        {
            var credentials = new Credentials
            {
                Username = request.Username,
                PasswordHash = _loginService.HashPassword(request.Password),
            };
            try
            {
                await _loginService.RegisterUserAsync(credentials);
                var token = _loginService.GenerateToken(credentials);
                return Ok(token);
            }
            catch (Exception _)
            {
                return StatusCode(422); // User already exists
            }
        }

        return StatusCode(501); // if somehow fails to register
    }
    
}
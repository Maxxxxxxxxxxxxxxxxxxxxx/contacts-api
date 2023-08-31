using ContactsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Controllers;

[ApiController]
[Route("api/login")]
public class AuthController : Controller
{
    private readonly LoginService _loginService;

    public AuthController(LoginService svc) => _loginService = svc;
    
    
}
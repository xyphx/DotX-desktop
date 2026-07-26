using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DotX.Api.DTOs;
using DotX.Api.Grpc.Clients;

namespace DotX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserGrpcClient _userGrpcClient;

    public AuthController(UserGrpcClient userGrpcClient)
    {
        _userGrpcClient = userGrpcClient;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.ApiKey))
        {
            return BadRequest("API Key is required");
        }

        try
        {
            var user = await _userGrpcClient.GetUserByApiKeyAsync(request.ApiKey);
            return Ok(new LoginResponse 
            { 
                Id = user.Id, 
                Email = user.Email, 
                Name = user.Name 
            });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}

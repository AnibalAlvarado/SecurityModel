using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityGateway.Dtos;
using SecurityGateway.Services.Interfaces;
using System.Security.Claims;

namespace SecurityGateway.Controllers
{
    [ApiController]
    [Route("security/api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public AuthController(IHttpClientFactory httpClientFactory, ITokenService tokenService)
        {
            _httpClient = httpClientFactory.CreateClient("securityService");
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/User/login", request);

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized(new { Message = "Usuario o contraseña inválidos" });
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<SecurityResponse>>();
            if (apiResponse == null || apiResponse.Data == null || !apiResponse.Success)
            {
                return Unauthorized(new { Message = "Error al validar usuario" });
            }
            var token = _tokenService.GenerateToken(
                 apiResponse.Data.UserId.ToString(),
                 apiResponse.Data.Username,
                 apiResponse.Data.Role
             );

            return Ok(new
            {
                Token = token,
                ExpiresIn = 180  // En segundos (3 minutos)
            });
        }

        [HttpPost("refreshToken")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult RefreshToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
                return Unauthorized(new { Message = "Token inválido o expirado." });

            var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null || username == null || role == null)
                return Unauthorized(new { Message = "No se pudieron obtener los datos del token." });

            var newToken = _tokenService.GenerateToken(userId, username, role);

            return Ok(new
            {
                Token = newToken,
                ExpiresIn = 180  // 3 minutos
            });
        }

    }

}

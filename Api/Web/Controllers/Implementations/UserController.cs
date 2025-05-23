using Business.Implementations;
using Business.Interfaces;
using Entity.Dtos;
using Entity.DTOs;
using Entity.Model;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : RepositoryController<User, UserDto>
    {
        private readonly IUserBusiness _business;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserBusiness business, ILogger<UserController> logger)
            : base(business)
        {
            _logger = logger;
            _business = business;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<UserResponseDto>(null,false,"Datos de inicio de sesión inválidos",ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                // Validar credenciales
                var user = await _business.ValidateUserAsync(loginRequest.Username, loginRequest.Password);

                // Si no se encuentra el usuario o las credenciales son incorrectas
                if (user == null)
                {
                    return Unauthorized(new ApiResponse<UserResponseDto>(null,false,"Nombre de usuario o contraseña incorrectos",null));
                }

                // Devolver la información del usuario autenticado
                return Ok(new ApiResponse<UserResponseDto>(user,true,"Inicio de sesión exitoso",null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la autenticación del usuario");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<UserResponseDto>(null,false,"Error interno del servidor durante la autenticación",null));
            }
        }
    }
}

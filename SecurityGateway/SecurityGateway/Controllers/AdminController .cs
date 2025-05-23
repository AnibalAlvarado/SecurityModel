using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecurityGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("secure-data")]
        [Authorize(Roles = "Administrador")] // 👈 Protegido por rol
        public IActionResult GetSecureData()
        {
            return Ok("Este endpoint solo lo ve un Administrador.");
        }
    }
}

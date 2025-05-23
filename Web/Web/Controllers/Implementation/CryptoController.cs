using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers.Interfaces;

namespace Web.Controllers.Implementation
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptoController : ControllerBase, ICryptoController
    {
        private readonly ICryptoBusiness _cryptoBusiness;

        public CryptoController(ICryptoBusiness cryptoBusiness)
        {
            _cryptoBusiness = cryptoBusiness;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportAsync()
        {
            await _cryptoBusiness.ImportFromApiAsync();
            return Ok(new { Message = "Data imported successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var cryptos = await _cryptoBusiness.GetAllAsync();
            return Ok(cryptos);
        }
    }
}

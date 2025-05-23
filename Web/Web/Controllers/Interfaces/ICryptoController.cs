using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Interfaces
{
    public interface ICryptoController
    {
        Task<IActionResult> ImportAsync();
        Task<IActionResult> GetAllAsync();
    }
}

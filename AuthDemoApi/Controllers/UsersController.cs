using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(new[] { "anna (Editor)", "bela (Admin)", "cecil (User)" });
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            return Ok(new { message = $"A(z) {id} azonosítójú felhasználót törölte: {User.Identity?.Name}" });
        }
    }
}

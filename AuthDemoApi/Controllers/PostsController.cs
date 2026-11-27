using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostsController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous] // Felülírja az osztályszintű [Authorize]-t: publikusan olvasható
        public IActionResult GetAllPosts()
        {
            return Ok(new[] { "1. cikk: Mi az a JWT?", "2. cikk: Stateless API-k" });
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")] // RBAC: Csak Editor vagy Admin hozhat létre cikket
        public IActionResult CreatePost([FromBody] string title)
        {
            return Ok(new { message = $"Cikk létrehozva ('{title}') a következő felhasználó által: {User.Identity?.Name}" });
        }
    }
}

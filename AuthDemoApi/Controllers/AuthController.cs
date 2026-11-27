using AuthDemoApi.Models;
using AuthDemoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthDemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _tokenService;

        public AuthController(JwtTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous] // Bárki meghívhatja bejelentkezés nélkül
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Szimulált felhasználó-ellenőrzés
            var user = request.Username switch
            {
                "anna" when request.Password == "1234" => (Id: "101", Role: "Editor"),
                "bela" when request.Password == "admin" => (Id: "102", Role: "Admin"),
                "cecil" when request.Password == "user" => (Id: "103", Role: "User"),
                _ => (Id: "", Role: "")
            };

            if (string.IsNullOrEmpty(user.Id))
            {
                return Unauthorized(new { message = "Hibás felhasználónév vagy jelszó." }); // 401
            }

            // Token generálása
            var tokenString = _tokenService.GenerateToken(user.Id, request.Username, user.Role);

            // Biztonságos HttpOnly Cookie beállítása (XSS és CSRF védelemmel)
            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true,                // JS-ből nem elérhető (XSS védelem)
                Secure = true,                  // Csak HTTPS felett küldhető
                SameSite = SameSiteMode.Strict, // Cross-site kéréseknél tiltott (CSRF védelem)
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            return Ok(new
            {
                message = $"Sikeres bejelentkezés: {request.Username}",
                role = user.Role,
                accessToken = tokenString
            });
        }

        [HttpGet("profile")]
        [Authorize] // Csak érvényes tokennel érhető el (szerepkörtől függetlenül)
        public IActionResult GetProfile()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { username, role });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return Ok(new { message = "Sikeres kijelentkezés, a süti törölve." });
        }
    }
}

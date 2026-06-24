using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ConectaStore.API.Data;
using ConectaStore.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ConectaStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly string _adminEmail;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
        _adminEmail = _config["Admin:Email"] ?? "admin@conecta.com";
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { mensagem = "E-mail já cadastrado." });

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = HashPassword(request.Senha),
            DataCadastro = DateTime.UtcNow
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        var token = GerarToken(usuario);
        return Ok(new { token, usuario = new { usuario.Id, usuario.Nome, usuario.Email, isAdmin = false } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (usuario == null || !VerifyPassword(request.Senha, usuario.SenhaHash))
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

        var isAdmin = usuario.Email == _adminEmail;
        var token = GerarToken(usuario, isAdmin);
        return Ok(new { token, usuario = new { usuario.Id, usuario.Nome, usuario.Email, isAdmin } });
    }

    private string GerarToken(Usuario usuario, bool isAdmin = false)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email)
        };

        if (isAdmin)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
        return Convert.ToHexString(salt) + ":" + Convert.ToHexString(hash);
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split(':');
        byte[] salt = Convert.FromHexString(parts[0]);
        byte[] storedHash = Convert.FromHexString(parts[1]);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(storedHash, hash);
    }
}

public class RegisterRequest
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Senha { get; set; }
}

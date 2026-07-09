using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using orgfindom.Server.Models;
using System.Security.Claims;
using System.Text;

namespace orgfindom.Server.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{

    private readonly DbSim _dbSim;
    private readonly IConfiguration _config;

    public LoginController(DbSim dbSim, IConfiguration config)
    {
        _dbSim = dbSim;
        _config = config;
    }
    
    //POST pra nenhum dado ficar na url
    [HttpPost("request")] // "api/login/request"
    public IActionResult LoginUser([FromBody] LoginRequest? loginData = null)
    {
        if(loginData == null) return BadRequest("Os dados enviados não São válidos"); 


        User? user = _dbSim.UsersTable.FirstOrDefault(u => u.UserName == loginData.UserName);

        if(user == null) return Unauthorized(new { message = "Usuário ou senha incorretos" });

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginData.Password, user.Password);

        if (!isPasswordValid)
        {
            return Unauthorized(new {message = "Usuário ou senha incorretos"});
        }

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? 
            throw new InvalidOperationException("ERRO CRÍTICO: A variável 'JWT_SECRET_KEY' não foi encontrada no ambiente.")
        ;
        byte[]? key = Encoding.ASCII.GetBytes(secretKey);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.RoleLevel.ToString()),  
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("familyId", user.FamilyId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(4),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        Microsoft.IdentityModel.Tokens.SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
        string? tokenString = tokenHandler.WriteToken(token);
        return Ok(new { Token = tokenString, Age = user.UserAge, message = "Logado com sucesso"});

    }
}

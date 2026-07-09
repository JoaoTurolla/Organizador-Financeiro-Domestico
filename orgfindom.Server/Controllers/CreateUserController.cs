using Microsoft.AspNetCore.Mvc;
using orgfindom.Server.Models;

namespace orgfindom.Server.Controllers;

[ApiController]
[Route("api/create")] 
public class CreateUserController : ControllerBase
{
    private readonly DbSim _dbSim;

    public CreateUserController(DbSim dbSim)
    {
        _dbSim = dbSim;
    }

    [HttpPost] // "api/create"
    public IActionResult CreateUser([FromBody] User? newUser = null)
    {
        if(newUser == null) return BadRequest("Dados inválidos para criação do usuário.");
        if(newUser.UserAge < 0) return BadRequest("Usuário não pode ter idade negativa.");

        //Envia pro banco de dados checa disponibilidade e cria ou não o usuário
        bool userCreated = _dbSim.AddUser(newUser);

        if(!userCreated)
        {
            return BadRequest(new { message = "Nome de usuário já está em uso" });
        }
        return Ok(new { message = "Usuário criado com sucesso" });
    }   
}
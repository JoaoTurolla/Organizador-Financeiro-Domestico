using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orgfindom.Server.Models;
using System.Security.Claims;

namespace orgfindom.Server.Controllers;

[ApiController]
[Route("api/user")]
public class UserDataController : ControllerBase
{   
    private readonly DbSim _dbSim;
    public UserDataController(DbSim dbSim)
    {
        _dbSim = dbSim;
    }

    [Authorize]
    [HttpDelete("{userName}")]
    public IActionResult DeleteUesr(string userName){
        string? currentUserName = User.FindFirst(ClaimTypes.Name)?.Value;
        string? currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        User? userToDelete = _dbSim.UsersTable.FirstOrDefault(u => u.UserName == userName);

        bool isSelf = false;
        bool isAdmin = false;
        if(currentUserName == userName)
        {
            isSelf = true;
        }
        if(currentUserRole == "1")
        {
            isAdmin = true;
        }

        if(!isSelf && !isAdmin)
        {
            return Forbid();
        }

        if (userToDelete == null)
        {
            return NotFound(new {message = "Usuário não encontrado"});
        }
        
        _dbSim.TransactionsTable.RemoveAll(t => t.UserId == userToDelete.Id);
        
        bool removed = _dbSim.RemoveUser(userName);


        if (!removed)
        {
            return NotFound(new {message = "Usuário não encontrado"});
        }

        return Ok(new {message =$"Usuário {userName} removido com sucesso"});
    }

    [Authorize]
    [HttpGet("family")]
    public IActionResult GetFamilyUsers()
    {
        string? familyIdClaim = User.FindFirst("familyId")?.Value;

        if(string.IsNullOrWhiteSpace(familyIdClaim) || !int.TryParse(familyIdClaim, out int familyId))
        {
            return BadRequest(new {message = "Token inválido ou sem família vinculada"});
        }
        
        var familyMembers = _dbSim.UsersTable
        .Where(u => u.FamilyId == familyId)
        .Select(u => new { id = u.Id, name = u.UserName })
        .ToList();

        return Ok(familyMembers);
    }

    // [HttpGet("={username}-{currentRoleLevel}")] a ideia maravilhosa que tive antes de pesquisar mais :)
    [Authorize] //Garantir que só quem tem permissão poderá ver
    [HttpGet("transaction/{targetUserId}")] // "api/user/transaction/{targetUserId}"
    public IActionResult GetTransactionsByUser(int targetUserId)
    {
        string? loggedInUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(loggedInUserIdClaim) || !int.TryParse(loggedInUserIdClaim, out int loggedInUserId))
        {
            return BadRequest(new {message = "Token inválido ou corrompido."});
        }
        
        bool isViewingOwnData = false;
        bool isFromSameFamily = false;

        if(loggedInUserId == targetUserId) isViewingOwnData = true;
        else if(_dbSim.ShareSameFamily(loggedInUserId, targetUserId)) isFromSameFamily = true;

        if(!isViewingOwnData && !isFromSameFamily)
        {
            return Forbid(); //HTTP 403
        }

        List<Transaction>? transactions = _dbSim.TransactionsTable.Where(t => t.UserId == targetUserId).ToList();

        return Ok(transactions);
    }





}

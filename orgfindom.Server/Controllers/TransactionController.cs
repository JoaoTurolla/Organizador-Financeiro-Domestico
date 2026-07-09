using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using orgfindom.Server.Models;
using System.Security.Claims;

namespace orgfindom.Server.Controllers;

[ApiController]
[Route("api/transaction")]
public class TransactionController : ControllerBase
{
    private readonly DbSim _dbSim;

    public TransactionController(DbSim dbSim)
    {
        _dbSim = dbSim;
    }

    [Authorize]
    [HttpPost("create")] // "api/transaction/create"
    public IActionResult CreateTranasction([FromBody] TransactionRequest request)
    {
        if(request == null) return BadRequest("Dados da transação inválidos");
        if(request.CashValue <= 0) return BadRequest(new {message = "Valores monetários devem ser positivos"});

        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int loggedInUserId))
        {
            return Unauthorized(new {message = "Token inválido ou expirado"});     
        }
        string? userFamilyIdClaim = User.FindFirst("familyId")?.Value;
        if(string.IsNullOrWhiteSpace(userFamilyIdClaim) || !int.TryParse(userFamilyIdClaim, out int loggedInUserFamilyId))
        {
            return Unauthorized( new {message = "Token Inválido ou expirado"});
        }       

        bool success = _dbSim.AddTransaction(loggedInUserId, loggedInUserFamilyId, request.CashValue, request.TypeOfTransaction, request.Description);

        if(!success) return BadRequest(new {message = "Não foi possível criar a transação"});

        return Ok(new {message = "Transação craida com sucesso."});
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult DeleteTransaction(int id)
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int loggedInUserId))
        {
            return Unauthorized(new {message = "Token inválido ou expirado"});     
        }
        Transaction? targetTransaction = _dbSim.TransactionsTable.FirstOrDefault(t => t.TransactionId == id);

        if(targetTransaction == null) return NotFound(new {message = "Transação não encontrada"});


        if(loggedInUserId != targetTransaction.UserId && !_dbSim.ShareSameFamily(loggedInUserId, targetTransaction.UserId))
        {
            return Unauthorized(new {message = "Essa transação não pertence ao usuário e nem a família do usuário"});
        }

        bool removed = _dbSim.RemoveTransaction(id);
        if (!removed) return NotFound(new {message = "Transação não encontrada"});

        return Ok(new {message = "Transação removida com sucesso"});
    }

    [Authorize]
    [HttpGet("family")]
    public IActionResult GetFamilyTransactions()
    {
        string? familyIdClaim = User.FindFirst("familyId")?.Value;
        
        if (string.IsNullOrWhiteSpace(familyIdClaim) || !int.TryParse(familyIdClaim, out int familyId) || familyId == -1)
        {
            return BadRequest(new { message = "Token inválido ou sem família vinculada." });
        }

        // Busca as transações de TODOS os usuários que pertencem a esta família
        var familyTransactions = _dbSim.TransactionsTable
            .Where(t => t.FamilyId == familyId)
            .ToList();

        return Ok(familyTransactions);
    }
}
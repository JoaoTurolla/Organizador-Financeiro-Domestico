namespace orgfindom.Server.Models;

public class TransactionRequest
{
    public float CashValue { get; set; }
    public string TypeOfTransaction { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
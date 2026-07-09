namespace orgfindom.Server.Models;

public class Transaction
{
    public int UserId { get; set; }
    public int FamilyId { get; set; }
    public int TransactionId { get; set; }
    public float CashValue { get; set; }
    public string TypeOfTransaction { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Transaction() { }

    public Transaction(int uid, int fid, float cashValue, string typeOT, string description)
    {
        if(cashValue <= 0) throw new ArgumentException("Valor monetário deve ser maior que zero");

        UserId = uid;
        FamilyId = fid;
        CashValue = cashValue;
        TypeOfTransaction = typeOT;
        Description = description;

    }
}

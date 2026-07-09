//Simula o que deveria ser feito pelo banco de dados pois não consegui terminar de implementar o real 
//encontrado no arquivo Db.cs totalmente comentado e sim, tá bem no começo porque eu fiz esse aqui primeiro
using System.Collections.Generic;
using System.Text.Json;

namespace orgfindom.Server.Models;

public class DbSim
{
    public List<User> UsersTable { get; set; } = new();
    public List<Transaction> TransactionsTable { get; set; } = new();
    private readonly string _dataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
    private readonly string _usersFilePath;
    private readonly string _transactionsfilePath;

    public DbSim()
    {
        _usersFilePath = Path.Combine(_dataFolderPath, "users.json");
        _transactionsfilePath = Path.Combine(_dataFolderPath, "transactions.json");

        if (!Directory.Exists(_dataFolderPath))
        {
            Directory.CreateDirectory(_dataFolderPath);
        }

        LoadData();
    }

    private void LoadData()
    {
        if (File.Exists(_usersFilePath))
        {
            string usersJson = File.ReadAllText(_usersFilePath);
            UsersTable = JsonSerializer.Deserialize<List<User>>(usersJson) ?? new List<User>();
        }

        if (File.Exists(_transactionsfilePath))
        {
            string transactionsJson = File.ReadAllText(_transactionsfilePath);
            TransactionsTable = JsonSerializer.Deserialize<List<Transaction>>(transactionsJson) ?? new List<Transaction>();
        }
    }

    private void SaveData()
    {
        var options = new JsonSerializerOptions {WriteIndented = true };

        File.WriteAllText(_usersFilePath, JsonSerializer.Serialize(UsersTable, options));
        File.WriteAllText(_transactionsfilePath, JsonSerializer.Serialize(TransactionsTable, options));
    }

    public bool ShareSameFamily(int userId1, int userId2)
    {
        User? user1 = UsersTable.FirstOrDefault(u => u.Id == userId1);
        User? user2 = UsersTable.FirstOrDefault(u => u.Id == userId2);

        if(user1 == null || user2 == null || user1.FamilyId == -1 || user2.FamilyId == -1)
        {
            return false;
        }

        if(user1.FamilyId == user2.FamilyId) return true;
        
        return false;
    }


    public bool AddUser(User newUser)
    {
        //Checa por usuário com mesmo nome para saber se já existe
        foreach (User u in UsersTable)
        {
            if(u.UserName == newUser.UserName) return false;
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
        newUser.Password = passwordHash;
        newUser.Id = UsersTable.Count + 1;

        UsersTable.Add(newUser);
        SaveData();
        return true;
    }

    public bool RemoveUser(string userNameToRemove)
    {
        for(int i = 0; i < UsersTable.Count; i++)
        {
            if(userNameToRemove == UsersTable[i].UserName)
            {
                UsersTable.RemoveAt(i);
                SaveData();
                return true;
            }
        }
        return false;
    
    }

    public bool AddTransaction(int userId, int fid, float cashValue, string type, string description)
    {
        //Todos esses dados já chegam perfeitamente tratados
        Transaction newTransaction = new Transaction( userId, fid, cashValue, type, description);

        newTransaction.TransactionId = TransactionsTable.Count + 1;

        TransactionsTable.Add(newTransaction);
        SaveData();
        return true;
    }

    public bool RemoveTransaction(int transactionId)
    {
        for(int i = 0; i < TransactionsTable.Count; i++)
        {
            if(transactionId == TransactionsTable[i].TransactionId)
            {
                TransactionsTable.RemoveAt(i);
                SaveData();
                return true;
            }
        }
        
        return false;
    }

}

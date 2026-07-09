namespace orgfindom.Server.Models;

public class User
{
    public int Id {get; set;}
    public int UserAge{get; set;} 
    public int FamilyId { get; set;} = 1;//Deve ser alterado no arquivo diretamente
    public int RoleLevel { get; set; } = 3;//Deve ser alterado no arquivo diretamente
    public string UserName {get; set;} = string.Empty;
    public string Password { get; set; } = string.Empty;

    public User() { }
    public User(int userAge, string userName, string usrPsswrd, int id = -1, int familyId = 1, int roleLevel = 3)
    {
        Id = id;
        FamilyId = familyId;
        UserAge = userAge;
        RoleLevel = roleLevel;
        UserName = userName;
        Password = usrPsswrd;

    }

}
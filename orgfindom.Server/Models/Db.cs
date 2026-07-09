// using System;
// using DotNetEnv;
// using Google.Protobuf.WellKnownTypes;
// using MySql.Data.MySqlClient;


// //Não utilizado no momento pois não foi possível terminar a implementação
// public static class Db
// {
//     private static readonly string 
//         host = Environment.GetEnvironmentVariable("DB_HOST") ?? "",
//         port = Environment.GetEnvironmentVariable("DB_PORT") ?? "",
//         db = Environment.GetEnvironmentVariable("DB_NAME") ?? "",
//         uid = Environment.GetEnvironmentVariable("DB_UID") ?? "",
//         password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? ""
//     ;

//     private static readonly string _connectionString = 
//         "Server=" + host + ";Port=" + port + ";Database="+ db +";Uid=" + uid +";Pwd="+ password +";"
//     ;

//     public static MySqlConnection CriarConexao()
//     {
//         return new MySqlConnection(_connectionString);
//     }
// }
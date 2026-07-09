// using MySql.Data.MySqlClient;
// using DotNetEnv;
// using orgfindom.Server.Models;

// var builder = WebApplication.CreateBuilder(args);

// Env.Load();

// var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "";

// // using (var conexao = Db.CriarConexao())
// // {
// //     string query = "INSERT INTO table (column) VALUES (123)";
// //     MySqlCommand comando = new MySqlCommand(query, conexao);
// //     comando.Parameters.AddWithValue("@column", "123");

// //     conexao.Open();
// //     comando.ExecuteNonQuery();
// // }


// builder.Services.AddOpenApi();
// builder.Services.AddCors(policy =>
// {
//     policy.AddDefaultPolicy(builder =>
//     {
//         builder.WithOrigins(frontendUrl)
//                .AllowAnyHeader()
//                .AllowAnyMethod();
//     });
// });

// var app = builder.Build();

// app.UseCors();

// // app.MapGet("/api/message", () => new { texto = "Hello World From The Backend!" });

// app.MapPost("/api/login", (LoginRequest request) =>
// {
//     if (request.Password == "1234")
//     {
//         return Results.Ok(new { message = "Login successful" });
//     }
//     else
//     {
//         return Results.BadRequest(new { message = "Invalid password" });
//     }
// });

// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// // app.UseHttpsRedirection();

// app.Run();

// public record LoginRequest(string userName, int age, string Password);

using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using orgfindom.Server.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? 
    throw new InvalidOperationException("ERRO CRÍTICO: A variável 'FRONTEND_URL' não foi encontrada no ambiente.")
;

var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? 
    throw new InvalidOperationException("ERRO CRÍTICO: A variável 'JWT_SECRET_KEY' não foi encontrada no ambiente.")
;

builder.Services.AddSingleton<DbSim>();

builder.Services.AddControllers();

// Configuração do JWT Bearer Authentication
byte[] key = Encoding.ASCII.GetBytes(secretKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false 
        };
    });

builder.Services.AddOpenApi();

builder.Services.AddCors(policy =>
{
    policy.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(frontendUrl)
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors(); // Usar políticas definidas

app.UseAuthentication(); // Lê o Token JWT
app.UseAuthorization();  // Checa se o usuário tem permissão para a rota solicitada

app.MapControllers(); // Encaminha a requisição aprovada para os Controllers

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
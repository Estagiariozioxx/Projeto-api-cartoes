
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MariaDB.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do Serilog
var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
Directory.CreateDirectory(logFolder);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()  // Define o n�vel m�nimo como Error
    .WriteTo.File(new CompactJsonFormatter(),
           Path.Combine(logFolder, "log-.json"),  // Armazena logs em formato JSON compacto
           retainedFileCountLimit: 10,  // Mant�m at� 10 arquivos de log
           rollingInterval: RollingInterval.Day)  // Rotaciona os arquivos diariamente
    .WriteTo.File(
           Path.Combine(logFolder, "log-.log"),  // Armazena logs em formato texto
           retainedFileCountLimit: 10,
           rollingInterval: RollingInterval.Day)
    .WriteTo.MariaDB(builder.Configuration.GetConnectionString("stringConexao"),
                   tableName: "Logs",
                   autoCreateTable: true)  // Armazena os logs no banco de dados MySQLh
   .CreateLogger();
// #endregion

#region Lendo o appsettings
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

string pathAppsettings = "appsettings.json";

if (env == "Development")
{
    pathAppsettings = "appsettings.Development.json";
}

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(pathAppsettings)
    .Build();

Environment.SetEnvironmentVariable("STRING_CONEXAO", config.GetSection("stringConexao").Value);
#endregion




#region Habilitando o uso do Swagger (OPENAPI)

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gateway de Pagamentos",
        Version = "v1",
        Description = $@"<h3>Api de <b>Pagamentos</b></h3>
                        <p>
                            Api para gerenciar pagamentos
                        </p>",
        Contact = new OpenApiContact
        {
            Name = "Suporte Unoeste",
            Email = string.Empty,
            Url = new Uri("https://www.unoeste.br"),
        },
    });
    // Definindo o esquema de seguran�a
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira o token JWT no formato **Bearer {token}**",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // Adiciona a seguran�a ao Swagger
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });


    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

#endregion
// Configura��o do JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("leonardo-lopes-secreta-trabalho-bank-chave")),
        ValidAudience = "Usu�rios da API",
        ValidIssuer = "Gateway api",
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("APIAuth", new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser().Build());
});

//Habilitar o uso do serilog.
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

#region IOC 

//adicionado ao IOC por requisi��o
builder.Services.AddScoped(typeof(Bank.Services.CartaoService));
builder.Services.AddScoped(typeof(Bank.Services.PagamentoService));
/*
builder.Services.AddScoped(typeof(IntroAPI.Services.PedidoService.VendaService));
builder.Services.AddScoped(typeof(IntroAPI.Services.PedidoService.ProdutoService));*/


//adicionar ao IOC inst�ncia �nicas (singleton)
builder.Services.AddSingleton<Bank.BD>(new Bank.BD(builder.Configuration.GetConnectionString("stringConexao")));


#endregion




var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    c.RoutePrefix = ""; //habilitar a p�gina inicial da API ser a doc.
    c.DocumentTitle = "Gerenciamento de Produtos - API V1";
});

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();




//Cont�m as Afirma��o (claims) do token.
//As Claimns s�o declara��es sobre uma entidade (geralmente o usu�rio) e metadados adicionais.
//S�o objetos do tipo �Chave/Valor� que definem uma afirma��o,
//como por exemplo, dizer que o nome do usu�rio � "Andr�".
var userClaims = new List<Claim>();
userClaims.Add(new Claim(ClaimTypes.Name, "leonardo.lopes")); //Claim padr�o
userClaims.Add(new Claim(ClaimTypes.Role, "Administrador")); //Claim padr�o
userClaims.Add(new Claim("id", "123456"));
userClaims.Add(new Claim("cpf", "11111111111")); //sens�vel (?)  / pode-se criptografar
userClaims.Add(new Claim("data", DateTime.Now.ToString()));
userClaims.Add(new Claim("email", "leo@hotmail.com"));
userClaims.Add(new Claim("contratoId", "11111111"));

//Juntar as Claims em um conjunto.
var identidade = new ClaimsIdentity(userClaims);

//Iniciando a cri���o do token
var handler = new JwtSecurityTokenHandler();

//Cria��o da Chave de Assinatura (Signing Key):
string minhaKey = "leonardo-lopes-secreta-trabalho-bank-chave"; // >= 32 caracteres
SecurityKey key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.ASCII.GetBytes(minhaKey));
SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


//criando um descriptor: Objeto que reune o cabe�alho, payload e assinatura do token.
var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
{
    Audience = "Usu�rios da API", //Quem vai usar o token, p�blico-alvo
    Issuer = "Gateway api", //Emisssor
    NotBefore = DateTime.Now, //Data de in�cio
    Expires = DateTime.Now.AddYears(1), //Data fim 
    Subject = identidade, // credenciais de assinatura + Claims
    SigningCredentials = signingCredentials //a chave para criptografar os dados
};

//Cria��o do Token JWT
var dadosToken = handler.CreateToken(tokenDescriptor);

//gerando o token (encripta e gera ao token)
string jwt = handler.WriteToken(dadosToken);
Console.Write(jwt);















//.......
app.Run();


using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MariaDB.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
Directory.CreateDirectory(logFolder);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()  // Define o nível mínimo como Error
    .WriteTo.File(new CompactJsonFormatter(),
           Path.Combine(logFolder, "log-.json"),  // Armazena logs em formato JSON compacto
           retainedFileCountLimit: 10,  // Mantém até 10 arquivos de log
           rollingInterval: RollingInterval.Day)  // Rotaciona os arquivos diariamente
    .WriteTo.File(
           Path.Combine(logFolder, "log-.log"),  // Armazena logs em formato texto
           retainedFileCountLimit: 10,
           rollingInterval: RollingInterval.Day)
    .WriteTo.MariaDB(builder.Configuration.GetConnectionString("stringConexao"),
                   tableName: "Logs",
                   autoCreateTable: true)  // Armazena os logs no banco de dados MySQL
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
        Title = "Gerenciamento da API...",
        Version = "v1",
        Description = $@"<h3>Título <b>da API</b></h3>
                        <p>
                            Alguma descrição....
                        </p>",
        Contact = new OpenApiContact
        {
            Name = "Suporte Unoeste",
            Email = string.Empty,
            Url = new Uri("https://www.unoeste.br"),
        },
    });


    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

#endregion

//Habilitar o uso do serilog.
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
/*
#region IOC 

//adicionado ao IOC por requisição
builder.Services.AddScoped(typeof(IntroAPI.Services.AlunoService));
builder.Services.AddScoped(typeof(IntroAPI.Services.PedidoService.VendaService));
builder.Services.AddScoped(typeof(IntroAPI.Services.PedidoService.ProdutoService));


//adicionar ao IOC instância únicas (singleton)
builder.Services.AddSingleton<Bank.BD>(new Bank.BD());


#endregion

*/


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    c.RoutePrefix = ""; //habilitar a página inicial da API ser a doc.
    c.DocumentTitle = "Gerenciamento de Produtos - API V1";
});

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();


//.......
app.Run();

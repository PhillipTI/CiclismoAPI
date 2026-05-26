using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CiclismoAPI.Services;


var builder = WebApplication.CreateBuilder(args);

// MongoDB string de conexão do appsettings.json
var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"];
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"];

// Controllers, registra os controllers da API (recursos)
builder.Services.AddControllers();
// Registro dos Services para injeção de dependência.
builder.Services.AddSingleton<ProdutoService>();
builder.Services.AddSingleton<PedidoService>();
builder.Services.AddSingleton<AuthService>();

// Documentação pelo Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "CiclismoAPI",
        Version = "v1",
        Description = "API REST para loja de equipamentos de ciclismo"
    });

    // AULA 7 - JWT: Adiciona o botão Authorize no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Digite: Bearer {seu token}"
    });

  c.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});
// Configuração da autenticação com token JWT.
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = false,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey!)),

            ValidateLifetime = true
        };
    });

// Autorização por perfil (RBAC) 
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CiclismoAPI v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseHttpsRedirection();

// AULA 7 - JWT: Autenticação SEMPRE antes de Autorização
app.UseAuthentication();
app.UseAuthorization();

// Define o index.html como página padrão
app.UseDefaultFiles();
// Serve os arquivos estáticos do frontend (HTML, CSS, JS)
app.UseStaticFiles();

app.MapControllers();

app.Run();
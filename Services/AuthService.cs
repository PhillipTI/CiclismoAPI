
using CiclismoAPI.Models;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CiclismoAPI.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<Usuario> _usuarios;
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _usuarios = database.GetCollection<Usuario>("usuarios");
            _configuration = configuration;
        }

        // Segurança / OWASP A02 Cryptographic Failures:
        // Nunca salvamos a senha em texto puro.
     
        public async Task<Usuario?> Registrar(string nome, string email, string senha)
        {
            // Verifica se o email já existe
            var usuarioExistente = await _usuarios
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();

            if (usuarioExistente != null) return null;

            var usuario = new Usuario
            {
                Nome = nome,
                Email = email,
                //Segurança: BCrypt faz o hash da senha antes de salvar
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
                Role = "cliente",
                CriadoEm = DateTime.UtcNow
            };

            await _usuarios.InsertOneAsync(usuario);
            return usuario;
        }

        // JWT: O login verifica a senha e gera o token.
        // O token contém: nome, email, role (restições/permissões) e prazo de expiração.
        public async Task<string?> Login(string email, string senha)
        {
            var usuario = await _usuarios
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();

            // Segurança: BCrypt verifica a senha contra o hash salvo
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
                return null;

            return GerarToken(usuario);
        }

        // JWT: Geração do token com Claims.
      
        private string GerarToken(Usuario usuario)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id!),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                // AULA 7 - JWT/RBAC: A Role no token define o que o usuário pode fazer
                new Claim(ClaimTypes.Role, usuario.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expirationHours),
                signingCredentials: credentials
            );

            // Retorna o token no formato: header.payload.signature
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Busca usuário por ID — usado pelos Controllers 
        public async Task<Usuario?> BuscarPorId(string id)
        {
            return await _usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();
        }
    }
}
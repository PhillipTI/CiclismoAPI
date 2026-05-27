using CiclismoAPI.DTOs;
using CiclismoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CiclismoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

//SOLID-D: Injeção de Dependência.
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

// POST /api/auth/registrar
        //Segurança: senha é hasheada no Service antes de salvar
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarDTO dto)
        {
            // Validação básica de entrada
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
                return BadRequest(new { mensagem = "Email e senha são obrigatórios" });

            var usuario = await _authService.Registrar(dto.Nome, dto.Email, dto.Senha);

            // Se o email já existe, o Service retorna null
            if (usuario == null)
                return BadRequest(new { mensagem = "Email já cadastrado" });

            //REST: 201 Created — recurso criado com sucesso
            return StatusCode(201, new { mensagem = "Usuário registrado com sucesso", id = usuario.Id });
        }

// POST /api/auth/login
        //JWT: gera e retorna o token para o cliente usar nas próximas requisições
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var token = await _authService.Login(dto.Email, dto.Senha);

            // 401 Unauthorized — credenciais inválidas
            if (token == null)
                return Unauthorized(new { mensagem = "Email ou senha inválidos" });

            // 200 OK — login bem-sucedido
            return Ok(new TokenResponseDTO
            {
                Token = token
            });
        }
    }
}
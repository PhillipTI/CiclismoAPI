using CiclismoAPI.DTOs;
using CiclismoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CiclismoAPI.Controllers
{
    /// <summary>
    /// Gerencia autenticação de usuários — registro e login com JWT.
    /// </summary>
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

        /// <summary>Registra um novo usuário na plataforma.</summary>
        /// <remarks>
        /// O perfil padrão é "cliente". Para promover a "admin", altere diretamente no MongoDB Atlas.
        ///
        ///     POST /api/Auth/registrar
        ///     {
        ///         "nome": "João Silva",
        ///         "email": "joao@email.com",
        ///         "senha": "minhasenha123"
        ///     }
        /// </remarks>
        /// <param name="dto">Dados do usuário: nome, email e senha.</param>
        /// <response code="201">Usuário registrado com sucesso.</response>
        /// <response code="400">Email já cadastrado ou dados inválidos.</response>

// POST /api/auth/registrar
        //Segurança: senha é hasheada no Service antes de salvar
        [HttpPost("registrar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>Realiza o login e retorna um token JWT.</summary>
        /// <remarks>
        /// O token retornado deve ser usado no header Authorization: Bearer {token}
        /// para acessar endpoints protegidos.
        ///
        ///     POST /api/Auth/login
        ///     {
        ///         "email": "joao@email.com",
        ///         "senha": "minhasenha123"
        ///     }
        /// </remarks>
        /// <param name="dto">Credenciais: email e senha.</param>
        /// <response code="200">Login bem-sucedido. Retorna o token JWT.</response>
        /// <response code="401">Email ou senha inválidos.</response>

// POST /api/auth/login
        //JWT: gera e retorna o token para o cliente usar nas próximas requisições
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
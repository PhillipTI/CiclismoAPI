using CiclismoAPI.DTOs;
using CiclismoAPI.Models;
using CiclismoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CiclismoAPI.Controllers
{
     /// <summary>
    /// Gerencia o catálogo de produtos da loja de ciclismo.
    /// Endpoints de leitura são públicos. Criação, edição e exclusão exigem perfil admin.
    /// </summary>
    
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoService _produtoService;

        public ProdutosController(ProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        /// <summary>Retorna todos os produtos disponíveis na loja.</summary>
        /// <response code="200">Lista de produtos retornada com sucesso.</response>

// GET /api/produtos
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> GetTodos()
        {
            var produtos = await _produtoService.BuscarTodos();
            return Ok(produtos);
        }

/// <summary>Busca um produto específico pelo ID.</summary>
        /// <param name="id">ID do produto no formato ObjectId do MongoDB (24 caracteres hexadecimais).</param>
        /// <response code="200">Produto encontrado e retornado.</response>
        /// <response code="404">Produto não encontrado.</response>

// GET /api/produtos/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPorId(string id)
        {
            var produto = await _produtoService.BuscarPorId(id);
            if (produto == null)
                return NotFound(new { mensagem = "Produto não encontrado" });
            return Ok(produto);
        }

/// <summary>Cadastra um novo produto no catálogo. Requer perfil admin.</summary>
        /// <remarks>
        ///     POST /api/Produtos
        ///     {
        ///         "nome": "Capacete MTB Pro",
        ///         "descricao": "Capacete para mountain bike com ventilação",
        ///         "categoria": "Segurança",
        ///         "preco": 299.90,
        ///         "estoque": 10
        ///     }
        /// </remarks>
        /// <param name="dto">Dados do produto a ser criado.</param>
        /// <response code="201">Produto criado com sucesso.</response>
        /// <response code="400">Dados inválidos — nome é obrigatório.</response>
        /// <response code="401">Token JWT não fornecido.</response>
        /// <response code="403">Usuário não tem perfil admin.</response>

// POST /api/produtos
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Criar([FromBody] ProdutoCriarDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                return BadRequest(new { mensagem = "Nome é obrigatório" });

            var produto = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Categoria = dto.Categoria,
                Preco = dto.Preco,
                Estoque = dto.Estoque
            };

            var criado = await _produtoService.Criar(produto);
            return StatusCode(201, criado);
        }

        /// <summary>Atualiza parcialmente um produto. Requer perfil admin.</summary>
/// <remarks>
/// Envie apenas os campos que deseja alterar. Campos não enviados permanecem inalterados.
///
///     PATCH /api/Produtos/{id}
///     {
///         "preco": 259.90
///     }
///
/// Ou para atualizar apenas o estoque:
///
///     PATCH /api/Produtos/{id}
///     {
///         "estoque": 15
///     }
/// </remarks>
/// <param name="id">ID do produto a ser atualizado parcialmente.</param>
/// <param name="dto">Campos a serem atualizados — apenas os campos enviados serão modificados.</param>
/// <response code="200">Produto atualizado parcialmente com sucesso.</response>
/// <response code="400">Nenhum campo válido enviado para atualização.</response>
/// <response code="404">Produto não encontrado.</response>
/// <response code="401">Token JWT não fornecido ou inválido.</response>
/// <response code="403">Usuário não tem perfil admin.</response>

// PATH /api/produtos/{id}
    [HttpPatch("{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
public async Task<IActionResult> Patch(string id, [FromBody] ProdutoPatchDTO dto)
{
    var existente = await _produtoService.BuscarPorId(id);
    if (existente == null)
        return NotFound(new { mensagem = "Produto não encontrado" });

    var atualizado = await _produtoService.Patch(id, dto);
    if (!atualizado)
        return BadRequest(new { mensagem = "Nenhum campo válido enviado para atualização" });

    return Ok(new { mensagem = "Produto atualizado parcialmente com sucesso" });
}

/// <summary>Atualiza completamente um produto existente. Requer perfil admin.</summary>
        /// <remarks>
        ///     PUT /api/Produtos/{id}
        ///     {
        ///         "nome": "Capacete MTB Pro V2",
        ///         "descricao": "Nova versão com ventilação aprimorada",
        ///         "categoria": "Segurança",
        ///         "preco": 349.90,
        ///         "estoque": 8
        ///     }
        /// </remarks>
        /// <param name="id">ID do produto a ser atualizado.</param>
        /// <param name="dto">Dados completos do produto atualizado.</param>
        /// <response code="200">Produto atualizado com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
        /// <response code="401">Token JWT não fornecido.</response>
        /// <response code="403">Usuário não tem perfil admin.</response>

// PUT /api/produtos/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Atualizar(string id, [FromBody] ProdutoAtualizarDTO dto)
        {
            var existente = await _produtoService.BuscarPorId(id);
            if (existente == null)
                return NotFound(new { mensagem = "Produto não encontrado" });

            var produto = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Categoria = dto.Categoria,
                Preco = dto.Preco,
                Estoque = dto.Estoque
            };

            await _produtoService.Atualizar(id, produto);
            return Ok(new { mensagem = "Produto atualizado com sucesso" });
        }

        /// <summary>Remove um produto do catálogo. Requer perfil admin.</summary>
        /// <param name="id">ID do produto a ser removido.</param>
        /// <response code="204">Produto removido com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
        /// <response code="401">Token JWT não fornecido.</response>
        /// <response code="403">Usuário não tem perfil admin.</response>

// DELETE /api/produtos/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Deletar(string id)
        {
            var existente = await _produtoService.BuscarPorId(id);
            if (existente == null)
                return NotFound(new { mensagem = "Produto não encontrado" });

            await _produtoService.Deletar(id);
            return NoContent();
        }
    }
}
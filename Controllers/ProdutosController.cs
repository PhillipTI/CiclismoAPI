using CiclismoAPI.DTOs;
using CiclismoAPI.Models;
using CiclismoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CiclismoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoService _produtoService;

        public ProdutosController(ProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        // GET /api/produtos
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var produtos = await _produtoService.BuscarTodos();
            return Ok(produtos);
        }

        // GET /api/produtos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(string id)
        {
            var produto = await _produtoService.BuscarPorId(id);
            if (produto == null)
                return NotFound(new { mensagem = "Produto não encontrado" });
            return Ok(produto);
        }

        // POST /api/produtos
        [HttpPost]
        [Authorize(Roles = "admin")]
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

        // PUT /api/produtos/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
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

        // DELETE /api/produtos/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
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
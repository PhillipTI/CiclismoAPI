using CiclismoAPI.DTOs;
using CiclismoAPI.Models;
using CiclismoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CiclismoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT: todos os endpoints exigem token válido
    public class PedidosController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public PedidosController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        private string GetUsuarioId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        // GET /api/pedidos
        [HttpGet]
        public async Task<IActionResult> GetMeusPedidos()
        {
            var usuarioId = GetUsuarioId();
            var pedidos = await _pedidoService.BuscarPorUsuario(usuarioId);
            return Ok(pedidos);
        }

        // GET /api/pedidos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(string id)
        {
            var usuarioId = GetUsuarioId();
            var pedido = await _pedidoService.BuscarPorId(id, usuarioId);
            if (pedido == null)
                return NotFound(new { mensagem = "Pedido não encontrado" });
            return Ok(pedido);
        }

        // POST /api/pedidos
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] PedidoCriarDTO dto)
        {
            var usuarioId = GetUsuarioId();

            var pedido = new Pedido
            {
                Itens = dto.Itens.Select(i => new ItemPedido
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            var criado = await _pedidoService.Criar(pedido, usuarioId);
            if (criado == null)
                return BadRequest(new { mensagem = "Um ou mais produtos não encontrados" });

            return StatusCode(201, criado);
        }

        // PUT /api/pedidos/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AtualizarStatus(string id, [FromBody] string novoStatus)
        {
            var atualizado = await _pedidoService.AtualizarStatus(id, novoStatus);
            if (!atualizado)
                return NotFound(new { mensagem = "Pedido não encontrado" });
            return Ok(new { mensagem = "Status atualizado com sucesso" });
        }

        // DELETE /api/pedidos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(string id)
        {
            var usuarioId = GetUsuarioId();
            var deletado = await _pedidoService.Deletar(id, usuarioId);
            if (!deletado)
                return NotFound(new { mensagem = "Pedido não encontrado" });
            return NoContent();
        }
    }
}
using CiclismoAPI.DTOs;
using CiclismoAPI.Models;
using CiclismoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CiclismoAPI.Controllers
{
    /// <summary>
    /// Gerencia os pedidos dos usuários autenticados.
    /// Todos os endpoints exigem autenticação JWT.
    /// Cada usuário acessa apenas seus próprios pedidos — proteção contra IDOR.
    /// </summary>
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

//GET /api/pedidos/todos (Admin)

        /// <summary>Retorna todos os pedidos de todos os usuários. Requer perfil admin.</summary>
        /// <response code="200">Lista completa de pedidos retornada.</response>
        /// <response code="401">Token JWT não fornecido.</response>
        /// <response code="403">Usuário não tem perfil admin.</response>
        
        [HttpGet("todos")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetTodos()
        {
            var pedidos = await _pedidoService.BuscarTodos();
            return Ok(pedidos);
        }       


        /// <summary>Retorna todos os pedidos do usuário autenticado.</summary>
        /// <remarks>
        /// Retorna apenas os pedidos do usuário logado — nunca pedidos de outros usuários.
        /// </remarks>
        /// <response code="200">Lista de pedidos retornada com sucesso.</response>
        /// <response code="401">Token JWT não fornecido ou inválido.</response>
        
// GET /api/pedidos
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> GetMeusPedidos()
        {
            var usuarioId = GetUsuarioId();
            var pedidos = await _pedidoService.BuscarPorUsuario(usuarioId);
            return Ok(pedidos);
        }

        /// <summary>Busca um pedido específico pelo ID.</summary>
        /// <param name="id">ID do pedido no formato ObjectId do MongoDB.</param>
        /// <response code="200">Pedido encontrado e retornado.</response>
        /// <response code="404">Pedido não encontrado ou não pertence ao usuário.</response>
        /// <response code="401">Token JWT não fornecido ou inválido.</response>

// GET /api/pedidos/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<IActionResult> GetPorId(string id)
        {
            var usuarioId = GetUsuarioId();
            var pedido = await _pedidoService.BuscarPorId(id, usuarioId);
            if (pedido == null)
                return NotFound(new { mensagem = "Pedido não encontrado" });
            return Ok(pedido);
        }
        /// <summary>Cria um novo pedido para o usuário autenticado.</summary>
        /// <remarks>
        /// O sistema valida automaticamente o estoque de cada produto.
        /// O total é calculado pelo servidor com base nos preços atuais.
        /// O estoque é decrementado após a criação do pedido.
        ///
        ///     POST /api/Pedidos
        ///     {
        ///         "itens": [
        ///             {
        ///                 "produtoId": "6a11f120ef624fa245292edd",
        ///                 "quantidade": 2
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="dto">Lista de itens com produtoId e quantidade.</param>
        /// <response code="201">Pedido criado com sucesso.</response>
        /// <response code="400">Produto não encontrado ou estoque insuficiente.</response>
        /// <response code="401">Token JWT não fornecido ou inválido.</response>

// POST /api/pedidos
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

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
                return BadRequest(new { mensagem = "Produto não encontrado ou fora de estoque" });

            return StatusCode(201, criado);
        }
        
    /// <summary>Atualiza parcialmente um pedido. Requer perfil admin.</summary>
    /// <remarks>
    /// Permite atualizar o status do pedido. Ao cancelar, o estoque é restaurado automaticamente.
    ///
    /// Status disponíveis: pendente, confirmado, enviado, entregue, cancelado.
    ///
    ///     PATCH /api/Pedidos/{id}
    ///     {
    ///         "status": "confirmado"
    ///     }
    /// </remarks>
    /// <param name="id">ID do pedido a ser atualizado.</param>
    /// <param name="dto">Campo status com o novo valor.</param>
    /// <response code="200">Pedido atualizado com sucesso.</response>
    /// <response code="404">Pedido não encontrado.</response>
    /// <response code="401">Token JWT não fornecido ou inválido.</response>
    /// <response code="403">Usuário não tem perfil admin.</response>

// PATCH /api/pedidos/{id}
        [HttpPatch("{id}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
public async Task<IActionResult> Patch(string id, [FromBody] PedidoPatchDTO dto)
        {
            if (dto.Status == null)
                return BadRequest(new { mensagem = "Nenhum campo válido enviado para atualização" });
        
            var atualizado = await _pedidoService.AtualizarStatus(id, dto.Status);
            if (!atualizado)
                return NotFound(new { mensagem = "Pedido não encontrado" });
        
            return Ok(new { mensagem = "Pedido atualizado com sucesso" });
        }

        /// <summary>Remove um pedido do histórico do usuário autenticado.</summary>
        /// <param name="id">ID do pedido a ser removido.</param>
        /// <response code="204">Pedido removido com sucesso.</response>
        /// <response code="404">Pedido não encontrado ou não pertence ao usuário.</response>
        /// <response code="401">Token JWT não fornecido ou inválido.</response>

// DELETE /api/pedidos/{id}
        
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Deletar(string id)
     {
        var usuarioId = GetUsuarioId();
        var pedido = await _pedidoService.BuscarPorId(id, usuarioId);
     
        if (pedido == null)
            return NotFound(new { mensagem = "Pedido não encontrado" });
     
        // Cliente só pode cancelar pedidos pendentes ou confirmados
        if (pedido.Status != "pendente" && pedido.Status != "confirmado")
            return BadRequest(new { mensagem = $"Pedido não pode ser cancelado. Status atual: {pedido.Status}" });
     
        await _pedidoService.Deletar(id, usuarioId);
        return NoContent();
      }
 }
}
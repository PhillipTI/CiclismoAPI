namespace CiclismoAPI.DTOs
{
    public class ItemPedidoDTO
    {
        public string ProdutoId { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }

    public class PedidoCriarDTO
    {
        public List<ItemPedidoDTO> Itens { get; set; } = new();
    }

    // PATCH DTO para Pedido — permite atualizar campos específicos
    public class PedidoPatchDTO
    {
    public string? Status { get; set; }
    }
    }
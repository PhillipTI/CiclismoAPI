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
}
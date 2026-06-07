namespace CiclismoAPI.DTOs
{
    // Criação de um produto
    public class ProdutoCriarDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
    }

    // Atualização de um produto
    public class ProdutoAtualizarDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
    }

    // PATCH DTO — todos os campos são opcionais 
public class ProdutoPatchDTO
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public string? Categoria { get; set; }
    public decimal? Preco { get; set; }
    public int? Estoque { get; set; }
}
}
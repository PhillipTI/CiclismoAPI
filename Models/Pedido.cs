using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CiclismoAPI.Models
{
    public class Pedido
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Segurança / OWASP A01 Broken Access Control (IDOR):
        [BsonRepresentation(BsonType.ObjectId)]
        public string UsuarioId { get; set; } = string.Empty;

        // NoSQL / Modelo de Documento:
        // Em bancos relacionais (SQL) x MongoDB
        public List<ItemPedido> Itens { get; set; } = new();

        public decimal Total { get; set; }

        // Status possíveis: "pendente", "confirmado", "enviado", "entregue"
        public string Status { get; set; } = "pendente";
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }

    public class ItemPedido
    {
        // NoSQL: Guardar o Id E o Nome do produto. Preservação do nome original no NoSQL
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProdutoId { get; set; } = string.Empty;
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
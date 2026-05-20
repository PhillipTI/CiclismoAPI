
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CiclismoAPI.Models
{
    public class Produto
    {
        // NoSQL: [BsonId], [BsonRepresentation] converte o ObjectId do MongoDB para string no C#        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // REST: Estes campos formam o "recurso" Produto da API REST.
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        // Rotas: uso de query string para filtrar coleções
        public string Categoria { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
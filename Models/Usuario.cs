using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CiclismoAPI.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Segurança / OWASP 
        public string SenhaHash { get; set; } = string.Empty;

        // JWT / Autorização RBAC (Bônus B do trabalho):
        public string Role { get; set; } = "cliente";

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
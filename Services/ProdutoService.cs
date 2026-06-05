using CiclismoAPI.Models;
using MongoDB.Driver;
using System.Net.Security;
using CiclismoAPI.DTOs;

namespace CiclismoAPI.Services
{
    public class ProdutoService
    {
        private readonly IMongoCollection<Produto> _produtos;

        // Construtor para uso real — recebe IConfiguration e cria a conexão...
        public ProdutoService(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];
        // Consertando o erro de sll: nao conecta com o MongoDB Atlas por causa do SSL, entao desabilitamos a validação do certificado
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.SslSettings = new SslSettings
            {
            ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true
            };
            var client = new MongoClient(settings);
            var database = client.GetDatabase(databaseName);
            _produtos = database.GetCollection<Produto>("produtos");
        }

        // Construtor para testes — recebe a coleção diretamente e permite injetar um Mock no lugar do MongoDB real
        public ProdutoService(IMongoCollection<Produto> produtos)
        {
            _produtos = produtos;
        }

        public async Task<List<Produto>> BuscarTodos()
        {
            return await _produtos.Find(_ => true).ToListAsync();
        }

        public async Task<Produto?> BuscarPorId(string id)
        {
            return await _produtos.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Produto> Criar(Produto produto)
        {
            produto.CriadoEm = DateTime.UtcNow;
            await _produtos.InsertOneAsync(produto);
            return produto;
        }

        public async Task<bool> Atualizar(string id, Produto produtoAtualizado)
        {
            produtoAtualizado.Id = id;
            var resultado = await _produtos.ReplaceOneAsync(p => p.Id == id, produtoAtualizado);
            return resultado.ModifiedCount > 0;
        }

    // PATCH /api/produtos/{id}
    // Atualiza apenas os campos enviados — campos null são ignorados
public async Task<bool> Patch(string id, ProdutoPatchDTO dto)
{
    var produto = await BuscarPorId(id);
    if (produto == null) return false;

    // Só atualiza os campos que foram enviados (não nulos)
    var updateDefinitions = new List<UpdateDefinition<Produto>>();

    if (dto.Nome != null)
        updateDefinitions.Add(Builders<Produto>.Update.Set(p => p.Nome, dto.Nome));
    if (dto.Descricao != null)
        updateDefinitions.Add(Builders<Produto>.Update.Set(p => p.Descricao, dto.Descricao));
    if (dto.Categoria != null)
        updateDefinitions.Add(Builders<Produto>.Update.Set(p => p.Categoria, dto.Categoria));
    if (dto.Preco != null)
        updateDefinitions.Add(Builders<Produto>.Update.Set(p => p.Preco, dto.Preco.Value));
    if (dto.Estoque != null)
        updateDefinitions.Add(Builders<Produto>.Update.Set(p => p.Estoque, dto.Estoque.Value));

    if (!updateDefinitions.Any()) return false;

    var update = Builders<Produto>.Update.Combine(updateDefinitions);
    var resultado = await _produtos.UpdateOneAsync(p => p.Id == id, update);
    return resultado.ModifiedCount > 0;
}

        public async Task<bool> Deletar(string id)
        {
            var resultado = await _produtos.DeleteOneAsync(p => p.Id == id);
            return resultado.DeletedCount > 0;
        }
    }
}
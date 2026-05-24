using CiclismoAPI.Models;
using MongoDB.Driver;

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
            var client = new MongoClient(connectionString);
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

        public async Task<bool> Deletar(string id)
        {
            var resultado = await _produtos.DeleteOneAsync(p => p.Id == id);
            return resultado.DeletedCount > 0;
        }
    }
}
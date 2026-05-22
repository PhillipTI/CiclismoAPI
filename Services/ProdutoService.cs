using CiclismoAPI.Models;
using MongoDB.Driver;

namespace CiclismoAPI.Services
{
    public class ProdutoService
    {
        private readonly IMongoCollection<Produto> _produtos;


        // Princípio D do SOLID:  injeção de dependência
        public ProdutoService(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            // "produtos" é o nome da coleção que será criada no MongoDB Atlas
            _produtos = database.GetCollection<Produto>("produtos");
        }


    //CRUD
        // GET /api/produtos
        public async Task<List<Produto>> BuscarTodos()
        {
            return await _produtos.Find(_ => true).ToListAsync();
        }

        // GET /api/produtos/{id}
        public async Task<Produto?> BuscarPorId(string id)
        {
            return await _produtos.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        // POST /api/produtos
        public async Task<Produto> Criar(Produto produto)
        {
            produto.CriadoEm = DateTime.UtcNow;
            await _produtos.InsertOneAsync(produto);
            return produto;
        }

        // PUT /api/produtos/{id}
        public async Task<bool> Atualizar(string id, Produto produtoAtualizado)
        {
            produtoAtualizado.Id = id;
            var resultado = await _produtos.ReplaceOneAsync(p => p.Id == id, produtoAtualizado);
            return resultado.ModifiedCount > 0;
        }

        // DELETE /api/produtos/{id}
        public async Task<bool> Deletar(string id)
        {
            var resultado = await _produtos.DeleteOneAsync(p => p.Id == id);
            return resultado.DeletedCount > 0;
        }
    }
}
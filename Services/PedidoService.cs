using CiclismoAPI.Models;
using MongoDB.Driver;
using System.Net.Security;

namespace CiclismoAPI.Services
{
    public class PedidoService
    {
        private readonly IMongoCollection<Pedido> _pedidos;
        private readonly IMongoCollection<Produto> _produtos;

        public PedidoService(IConfiguration configuration)
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

            _pedidos = database.GetCollection<Pedido>("pedidos");

            // Verificação da existência do pedido antes de adicionar
            _produtos = database.GetCollection<Produto>("produtos");
        }

// GET /api/pedidos evitando o IDOR (ler pedidos que nao o do usuario logado):
   
        public async Task<List<Pedido>> BuscarPorUsuario(string usuarioId)
        {
            return await _pedidos.Find(p => p.UsuarioId == usuarioId).ToListAsync();
        }

// GET /api/pedidos/{id}

        public async Task<Pedido?> BuscarPorId(string id, string usuarioId)
        {
            return await _pedidos
                .Find(p => p.Id == id && p.UsuarioId == usuarioId)
                .FirstOrDefaultAsync();
        }

// POST /api/pedidos: verificação e criação, soma total, etc...
        
        public async Task<Pedido?> Criar(Pedido pedido, string usuarioId)
{
    pedido.UsuarioId = usuarioId;
    pedido.CriadoEm = DateTime.UtcNow;
    pedido.Status = "pendente";

    decimal total = 0;
    foreach (var item in pedido.Itens)
    {
        var produto = await _produtos
            .Find(p => p.Id == item.ProdutoId)
            .FirstOrDefaultAsync();

        if (produto == null) return null;

        //Incluído: Validação de negócio: verifica se há estoque suficiente 
        if (produto.Estoque < item.Quantidade) return null;

        item.NomeProduto = produto.Nome;
        item.PrecoUnitario = produto.Preco;
        total += produto.Preco * item.Quantidade;
    }

    pedido.Total = total;
    await _pedidos.InsertOneAsync(pedido);

    // Atualiza o estoque de cada produto após criar o pedido
    foreach (var item in pedido.Itens)
    {
        var update = Builders<Produto>.Update
            .Inc(p => p.Estoque, -item.Quantidade);
        await _produtos.UpdateOneAsync(p => p.Id == item.ProdutoId, update);
    }

    return pedido;
}
  
// PUT /api/pedidos/{id}/status: apenas admin pode atualizar o status do pedido.
       
    public async Task<bool> AtualizarStatus(string id, string novoStatus)
{
    var pedido = await _pedidos.Find(p => p.Id == id).FirstOrDefaultAsync();
    if (pedido == null) return false;

    var statusAnterior = pedido.Status;

    var update = Builders<Pedido>.Update.Set(p => p.Status, novoStatus);
    var resultado = await _pedidos.UpdateOneAsync(p => p.Id == id, update);

    // Se o pedido foi cancelado, devolve os itens ao estoque
    if (novoStatus.ToLower() == "cancelado" && 
        statusAnterior.ToLower() != "cancelado")
    {
        foreach (var item in pedido.Itens)
        {
            var estoqueUpdate = Builders<Produto>.Update
                .Inc(p => p.Estoque, item.Quantidade);
            await _produtos.UpdateOneAsync(
                p => p.Id == item.ProdutoId, estoqueUpdate);
        }
    }

    return resultado.ModifiedCount > 0;
}

// DELETE /api/pedidos/{id}
        public async Task<bool> Deletar(string id, string usuarioId)
        {
            var resultado = await _pedidos
                .DeleteOneAsync(p => p.Id == id && p.UsuarioId == usuarioId);
            return resultado.DeletedCount > 0;
        }
    }
}
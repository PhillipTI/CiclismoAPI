using CiclismoAPI.Models;
using CiclismoAPI.Services;
using MongoDB.Driver;
using Moq;

namespace CiclismoAPI.Tests
{
    public class ProdutoServiceTests
    {
        // Mock da coleção MongoDB 
        private readonly Mock<IMongoCollection<Produto>> _mockColecao;
        private readonly ProdutoService _service;

        public ProdutoServiceTests()
        {
            _mockColecao = new Mock<IMongoCollection<Produto>>();

            _service = new ProdutoService(_mockColecao.Object);
        }

        // TESTE 1: Criar produto com dados válidos
        [Fact]
        public async Task Criar_ProdutoValido_RetornaProdutoComId()
        {
            var produto = new Produto
            {
                Nome = "Capacete MTB",
                Descricao = "Capacete para mountain bike",
                Categoria = "Segurança",
                Preco = 299.90m,
                Estoque = 10
            };

            _mockColecao
                .Setup(c => c.InsertOneAsync(
                    It.IsAny<Produto>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var resultado = await _service.Criar(produto);

            // Assert — verifica o resultado: se o comportamento está correto
            Assert.NotNull(resultado);
            Assert.Equal("Capacete MTB", resultado.Nome);
            Assert.Equal(299.90m, resultado.Preco);
        }

        // TESTE 2: Buscar produto por ID existente
        [Fact]
        public async Task BuscarPorId_IdExistente_RetornaProduto()
        {
            var produtoEsperado = new Produto
            {
                Id = "6a11f120ef624fa245292edd",
                Nome = "Luva de Ciclismo",
                Preco = 89.90m
            };

            var mockCursor = new Mock<IAsyncCursor<Produto>>();
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            mockCursor
                .Setup(c => c.Current)
                .Returns(new List<Produto> { produtoEsperado });

            _mockColecao
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Produto>>(),
                    It.IsAny<FindOptions<Produto, Produto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var resultado = await _service.BuscarPorId("6a11f120ef624fa245292edd");

            Assert.NotNull(resultado);
            Assert.Equal("Luva de Ciclismo", resultado.Nome);
        }


        // TESTE 3: Buscar produto por ID inexistente — retorna null
        [Fact]
        public async Task BuscarPorId_IdInexistente_RetornaNull()
        {
            var mockCursor = new Mock<IAsyncCursor<Produto>>();
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            mockCursor
                .Setup(c => c.Current)
                .Returns(new List<Produto>());

            _mockColecao
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<Produto>>(),
                    It.IsAny<FindOptions<Produto, Produto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            var resultado = await _service.BuscarPorId("id_que_nao_existe");

            // Segurança: produto não encontrado, retornando null
            Assert.Null(resultado);
        }

        // TESTE 4: Deletar produto inexistente — retorna false
        [Fact]
        public async Task Deletar_IdInexistente_RetornaFalse()
        {
            var mockResultado = new Mock<DeleteResult>();
            mockResultado
                .Setup(r => r.DeletedCount)
                .Returns(0);

            _mockColecao
                .Setup(c => c.DeleteOneAsync(
                    It.IsAny<FilterDefinition<Produto>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResultado.Object);

            var resultado = await _service.Deletar("id_inexistente");

            Assert.False(resultado);
        }
    }
}
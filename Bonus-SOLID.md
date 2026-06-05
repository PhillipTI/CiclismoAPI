# Princípios SOLID Aplicados — CiclismoAPI

> Documentação dos princípios SOLID aplicados no backend da CiclismoAPI,
> conforme exigido pelo Bônus D do trabalho prático semestral.

---

## S — Single Responsibility Principle
**Princípio da Responsabilidade Única**

**Arquivos:** `Services/ProdutoService.cs`, `Services/PedidoService.cs`, `Services/AuthService.cs`

Cada classe tem uma única responsabilidade bem definida:

| Classe | Responsabilidade única |
|--------|----------------------|
| `ProdutoService` | Operações de Produto no MongoDB (CRUD + PATCH) |
| `PedidoService` | Operações de Pedido + validação e controle de estoque |
| `AuthService` | Autenticação: hash de senha (BCrypt) + geração de JWT |
| `ProdutosController` | Receber requisições HTTP e retornar respostas REST |
| `AuthController` | Endpoints de login e registro |

**Justificativa:** Se precisarmos mudar como os produtos são salvos no banco,
alteramos apenas o `ProdutoService` — sem tocar nos controllers, DTOs ou models.
Se a regra de negócio do estoque mudar, alteramos apenas o `PedidoService`.
Cada classe muda por apenas um motivo.

---

## O — Open/Closed Principle
**Princípio Aberto/Fechado**

**Arquivo:** `Program.cs`

Os Services estão abertos para extensão mas fechados para modificação.
Para adicionar um novo recurso (ex: `CategoriaService`), basta:

1. Criar o novo arquivo `Services/CategoriaService.cs`
2. Registrar em `Program.cs`:

```csharp
builder.Services.AddSingleton<CategoriaService>();
```

Nenhum Service existente precisa ser modificado. O mesmo vale para Controllers
e DTOs — adicionamos `ProdutoPatchDTO` e `PedidoPatchDTO` sem alterar os DTOs
existentes de criação e atualização.

**Justificativa:** Durante o desenvolvimento adicionamos PATCH aos produtos e
pedidos criando novos métodos e DTOs sem modificar o comportamento já existente
e testado dos métodos `Criar`, `Atualizar` e `Deletar`.

---

## L — Liskov Substitution Principle
**Princípio da Substituição de Liskov**

**Arquivos:** `Controllers/ProdutosController.cs`, `Controllers/PedidosController.cs`,
`Controllers/AuthController.cs`

Todos os Controllers herdam de `ControllerBase` e podem ser tratados de forma
uniforme pelo pipeline do ASP.NET Core. O framework não precisa conhecer cada
Controller específico — trata todos como `ControllerBase`.

```csharp
public class ProdutosController : ControllerBase { }
public class PedidosController  : ControllerBase { }
public class AuthController     : ControllerBase { }
```

**Justificativa:** Qualquer Controller pode substituir outro no pipeline sem
quebrar o comportamento esperado — o ASP.NET Core roteia requisições, injeta
dependências e processa respostas da mesma forma para todos, independente da
implementação específica de cada um.

---

## I — Interface Segregation Principle
**Princípio da Segregação de Interfaces**

**Arquivos:** `DTOs/AuthDTOs.cs`, `DTOs/ProdutoDTOs.cs`, `DTOs/PedidoDTOs.cs`

Em vez de um único objeto grande com todos os campos possíveis, criamos DTOs
específicos e enxutos para cada operação:

| DTO | Campos | Para que serve |
|-----|--------|----------------|
| `RegistrarDTO` | Nome, Email, Senha | Apenas registro |
| `LoginDTO` | Email, Senha | Apenas login |
| `TokenResponseDTO` | Token | Apenas resposta do login |
| `ProdutoCriarDTO` | Nome, Desc, Cat, Preco, Estoque | Apenas criação |
| `ProdutoAtualizarDTO` | Nome, Desc, Cat, Preco, Estoque | Apenas PUT completo |
| `ProdutoPatchDTO` | Nome?, Desc?, Cat?, Preco?, Estoque? | Apenas PATCH parcial |
| `PedidoCriarDTO` | Itens | Apenas criação de pedido |
| `PedidoPatchDTO` | Status? | Apenas atualização de status |

**Justificativa:** O `ProdutoPatchDTO` tem todos os campos **nullable** (`string?`,
`decimal?`, `int?`) porque no PATCH apenas os campos enviados são atualizados.
Isso seria impossível com um único DTO compartilhado — o `ProdutoCriarDTO` tem
campos obrigatórios. Cada interface (DTO) expõe somente o contrato necessário
para aquela operação específica.

---

## D — Dependency Inversion Principle
**Princípio da Inversão de Dependência**

**Arquivos:** `Program.cs`, `Services/ProdutoService.cs`, `Services/AuthService.cs`,
`Services/PedidoService.cs`, `CiclismoAPI.Tests/ProdutoServiceTests.cs`

Os módulos de alto nível (Controllers) não dependem dos módulos de baixo nível
(Services) diretamente — ambos dependem de abstrações.

**No código de produção:**
```csharp
// Program.cs — o .NET gerencia a criação e entrega das dependências
builder.Services.AddSingleton<ProdutoService>();

// ProdutosController — recebe o Service pronto, não o cria
public ProdutosController(ProdutoService produtoService)
{
    _produtoService = produtoService;
}
```

**Nos testes unitários — benefício direto do D:**

O `ProdutoService` tem dois construtores: um para produção (recebe `IConfiguration`)
e um para testes (recebe `IMongoCollection<Produto>` diretamente). Isso permite
injetar um Mock no lugar do MongoDB real:

```csharp
// Construtor de teste — recebe a coleção por injeção
public ProdutoService(IMongoCollection<Produto> produtos)
{
    _produtos = produtos;
}

// Nos testes — Mock substitui o MongoDB real
var mockColecao = new Mock<IMongoCollection<Produto>>();
var service = new ProdutoService(mockColecao.Object);
```

**Justificativa:** A inversão de dependência foi o que tornou os testes unitários
possíveis sem precisar de conexão com o MongoDB Atlas. O Service depende da
abstração `IMongoCollection<Produto>`, não de uma implementação concreta —
permitindo substituir por um Mock em tempo de teste.

---

## Resumo

| Princípio | Aplicado em | Benefício obtido |
|-----------|-------------|-----------------|
| **S** | Services separados por entidade | Manutenção isolada sem efeitos colaterais |
| **O** | Novos DTOs e Services sem modificar existentes | PATCH adicionado sem quebrar CRUD existente |
| **L** | Controllers herdam ControllerBase | Pipeline ASP.NET funciona uniformemente |
| **I** | DTOs específicos por operação | PatchDTO com campos nullable sem afetar outros DTOs |
| **D** | Injeção de dependência nos Services | Testes unitários com Mock sem banco real |

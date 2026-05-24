Princípios SOLID Aplicados:

+S — Single Responsibility Principle:

Arquivo: "ProdutoService.cs", "PedidoService.cs", "AuthService.cs"

Cada classe tem uma única responsabilidade. O "ProdutoService" é responsável exclusivamente pelas operações de Produto no MongoDB; O "AuthService" cuida apenas de autenticação e o "PedidoService" gerencia apenas pedidos.
Se precisarmos mudar como os produtos são salvos no banco, alteramos apenas o "ProdutoService" — sem tocar em nada mais.

+O — Open/Closed Principle
Arquivo: "ProdutoService.cs", "PedidoService.cs", "AuthService.cs"

Os Services estão abertos para extensão mas fechados para modificação. Para adicionar um novo recurso (ex: "CategoriaService"), criamos um novo arquivo sem modificar os Services existentes. O "Program.cs" registra o novo Service via "builder.Services.AddSingleton<CategoriaService>()" sem alterar o restante da configuração.

+L — Liskov Substitution Principle
Arquivo: "ProdutosController.cs", "PedidosController.cs", "AuthController.cs"

Todos os Controllers herdam de "ControllerBase" e podem ser substituídos entre si sem quebrar o comportamento esperado do pipeline do ASP.NET.
O framework trata todos os Controllers de forma uniforme — registrando rotas, injetando dependências e processando requisições da mesma forma independente do Controller específico.

+I — Interface Segregation Principle
Arquivo: "AuthDTOs.cs", "ProdutoDTOs.cs", "PedidoDTOs.cs"

Em vez de um único objeto grande com vários campos , criamos DTOs específicos para cada operação.
O "LoginDTO" tem apenas "Email" e "Senha". O "RegistrarDTO" tem "Nome", "Email" e "Senha". O "ProdutoCriarDTO" tem apenas os campos necessários para criação.
Cada DTO expõe somente o que aquela operação precisa — sem campos desnecessários.

+D — Dependency Inversion Principle
Arquivo: "Program.cs", "ProdutoService.cs", "AuthService.cs"

Os "Services" dependem de abstrações ("IConfiguration"), não de implementações concretas.
O "ProdutoService" não cria sua própria configuração — ela é injetada pelo .NET via "builder.Services.AddSingleton<ProdutoService>()".
Os Controllers não criam os Services — recebem via injeção de dependência no construtor.
Isso permite trocar implementações sem alterar as classes dependentes.

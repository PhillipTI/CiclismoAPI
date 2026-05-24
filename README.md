Arquitetura de Aplicações Web — 2026.1
Professor: Thalles Noce
Aluno: Phillip Silva
Curso: ADS - Semi Presencial

Esta API REST foi construida para uma loja de venda de equipamentos de ciclismo (intitulada: CiclismoShop).

-Descrição do Projeto:

A CiclismoShop é uma aplicação web para venda de equipamentos de ciclismo. O sistema permite o cadastro de produtos, registro e autenticação de usuários e realização de pedidos com persistência em banco de dados NoSQL (MongoDB Atlas).

-Domínio:

Loja de equipamentos de ciclismo (capacetes, bicicletas, acessórios, vestuário).

-Principais entidades:

Produtos — itens disponíveis para venda;
Usuários — clientes e administradores da loja;
Pedidos — compras realizadas pelos usuários;

-Tecnologias Utilizadas:

Backend: .NET 10 / ASP.NET Core;  
Banco de Dados: MongoDB Atlas (NoSQL);  
Autenticação: JWT (JSON Web Token);  
Documentação: Swagger / OpenAPI 3.0;  
Frontend: HTML + JavaScript (Fetch API);

-Pré-requisitos (Instalações necessárias):

*.NET 10 SDK: (https://dotnet.microsoft.com/download/dotnet/10.0);
*Git: (https://git-scm.com/);
*Conta no MongoDB Atlas: (https://www.mongodb.com/cloud/atlas) (gratuito);
*Postman: (https://www.postman.com/downloads/) (para testar endpoints protegidos);

-Executando Localmente:

1 - Clone o repositório:

(bash)
git clone https://github.com/PhillipTI/CiclismoAPI.git
cd CiclismoAPI

2 - Configure as variáveis de ambiente:

Crie o arquivo "appsettings.Development.json" na raiz do projeto com o seguinte conteúdo:

(json)
{
"MongoDB": {
"ConnectionString": "SUA_STRING_DE_CONEXAO_MONGODB",
"DatabaseName": "CiclismoAPI"
},
"Jwt": {
"SecretKey": "SUA_SECRET_KEY_COM_MINIMO_32_CARACTERES",
"Issuer": "CiclismoAPI",
"ExpirationHours": 8
}
}

Obs.: O arquivo contendo as credenciais estão guardadas em um outro (.gitignore) por questões de proteção do dados sensíveis.

3 - Instale as dependências:

(bash)
dotnet restore

4 - Execute o projeto:

(bash)
dotnet run

5 - Acesse o frontend:

Abra o navegador em: http://localhost:5067.

-Documentação Swagger:

Com o projeto rodando, acesse: http://localhost:5067/swagger.

obs.: para testes de endpoints protegidos (criar produto, pedidos), utilize o "Postman" com o header "Authorization: Bearer {token}".

-Variáveis de Ambiente (Variável -- Descrição -- Exemplo)

       Variável                          Descrição                                              Exemplo

"MongoDB:ConnectionString" String de conexão do MongoDB Atlas "mongodb+srv://usuario:senha@cluster.mongodb.net/"
"MongoDB:DatabaseName" Nome do banco de dados "CiclismoAPI"  
"Jwt:SecretKey" Chave secreta para assinar os tokens JWT "MinhaChaveSecreta2026ComMaisDe32Caracteres"  
"Jwt:Issuer" Identificador do emissor do token "CiclismoAPI"  
"Jwt:ExpirationHours" Tempo de expiração do token em horas "8"

-Endpoints da API:

Auth:

| Método | Rota                  | Descrição              | Auth |
| ------ | --------------------- | ---------------------- | ---- |
| POST   | "/api/Auth/registrar" | Registrar novo usuário | ❌   |
| POST   | "/api/Auth/login"     | Login e geração do JWT | ❌   |

Produtos:

| Método | Rota                 | Descrição                | Auth     |
| ------ | -------------------- | ------------------------ | -------- |
| GET    | "/api/Produtos"      | Listar todos os produtos | ❌       |
| GET    | "/api/Produtos/{id}" | Buscar produto por ID    | ❌       |
| POST   | "/api/Produtos"      | Criar produto            | ✅ Admin |
| PUT    | "/api/Produtos/{id}" | Atualizar produto        | ✅ Admin |
| DELETE | "/api/Produtos/{id}" | Deletar produto          | ✅ Admin |

Pedidos:

| Método | Rota                       | Descrição            | Auth     |
| ------ | -------------------------- | -------------------- | -------- |
| GET    | "/api/Pedidos"             | Listar meus pedidos  | ✅ User  |
| GET    | "/api/Pedidos/{id}"        | Buscar pedido por ID | ✅ User  |
| POST   | "/api/Pedidos"             | Criar pedido         | ✅ User  |
| PUT    | "/api/Pedidos/{id}/status" | Atualizar status     | ✅ Admin |
| DELETE | "/api/Pedidos/{id}"        | Deletar pedido       | ✅ User  |

-Como Testar com Postman:

1 - Registrar usuário:
POST http://localhost:5067/api/Auth/registrar
Body: { "nome": "Seu Nome", "email": "email@email.com", "senha": "suasenha" }

2 - Fazer login e copiar o token:
POST http://localhost:5067/api/Auth/login
Body: { "email": "email@email.com", "senha": "suasenha" }

3 - Usar o token nas requisições protegidas:
Authorization: Bearer {token copiado}

-Perfis de Usuário (RBAC):

"cliente": Ver produtos, criar e visualizar próprios pedidos;  
"admin": Tudo do cliente + criar, editar e deletar produtos;
atualizar status de pedidos;

Obs.: o perfil padrão ao registrar é "cliente". Para promover a "admin", altere o campo "role" diretamente no MongoDB Atlas.

-Estrutura do Projeto:

CiclismoAPI/
├── Controllers/ # Endpoints REST da API
├── DTOs/ # Objetos de transferência de dados
├── Models/ # Entidades do domínio
├── Services/ # Regras de negócio e acesso ao banco
├── wwwroot/ # Frontend (HTML + JavaScript)
├── appsettings.json # Configurações gerais
└── Program.cs # Configuração e inicialização da aplicação

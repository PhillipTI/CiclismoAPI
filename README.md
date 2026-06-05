# 🚴 CiclismoAPI

> API REST completa para uma loja de equipamentos de ciclismo, desenvolvida como trabalho prático semestral da disciplina **Arquitetura de Aplicações Web — 2026.1**.

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com)
[![MongoDB](https://img.shields.io/badge/MongoDB-Atlas-green)](https://www.mongodb.com/cloud/atlas)
[![JWT](https://img.shields.io/badge/Auth-JWT-orange)](https://jwt.io)
[![Swagger](https://img.shields.io/badge/Docs-Swagger-brightgreen)](http://localhost:5067/swagger)

---

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Execução](#instalação-e-execução)
- [Variáveis de Ambiente](#variáveis-de-ambiente)
- [Documentação da API](#documentação-da-api)
- [Endpoints](#endpoints)
- [Autenticação JWT](#autenticação-jwt)
- [Perfis de Usuário RBAC](#perfis-de-usuário-rbac)
- [Frontend](#frontend)
- [Testes Unitários](#testes-unitários)
- [Docker](#docker)
- [Princípios SOLID](#princípios-solid)
- [Estrutura do Projeto](#estrutura-do-projeto)

---

## 📖 Sobre o Projeto

A **CiclismoShop** é uma aplicação web completa de e-commerce para venda de equipamentos de ciclismo. O sistema permite cadastro e gerenciamento de produtos, autenticação de usuários com diferentes níveis de acesso e realização de pedidos com controle automático de estoque.

**Domínio:** Loja de equipamentos de ciclismo — capacetes, bicicletas, roupas, acessórios e peças.

**Entidades principais:**

| Entidade | Descrição |
|----------|-----------|
| `Produto` | Itens disponíveis para venda com controle de estoque |
| `Usuario` | Clientes e administradores da plataforma |
| `Pedido` | Compras realizadas com cálculo automático de total |

---

## 🛠️ Tecnologias

| Camada | Tecnologia | Versão |
|--------|-----------|--------|
| Backend | .NET / ASP.NET Core | 10.0 |
| Banco de Dados | MongoDB Atlas (NoSQL) | Cloud |
| Autenticação | JWT Bearer + BCrypt | — |
| Documentação | Swagger / OpenAPI | 3.0 |
| Frontend | HTML + JavaScript (Fetch API) | — |
| Containerização | Docker (Multi-Stage Build) | — |
| Testes | xUnit + Moq | — |

---

## 🏗️ Arquitetura

```
Requisição HTTP
      ↓
  Controller       ← recebe, valida e retorna resposta HTTP
      ↓
  Service          ← regras de negócio e acesso ao MongoDB
      ↓
  MongoDB Atlas    ← persistência na nuvem
      ↓
  DTO              ← formata a resposta sem expor dados internos
```

**Padrão adotado:** separação em camadas (Controllers → Services → Models/DTOs), seguindo os princípios SOLID e boas práticas REST.

---

## ✅ Pré-requisitos

Antes de executar o projeto, instale:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Git](https://git-scm.com/)
- Conta gratuita no [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
- [Postman](https://www.postman.com/downloads/) — para testar endpoints protegidos

---

## 🚀 Instalação e Execução

### 1. Clone o repositório

```bash
git clone https://github.com/PhillipTI/CiclismoAPI.git
cd CiclismoAPI
```

### 2. Configure as variáveis de ambiente

Crie o arquivo `appsettings.Development.json` na raiz do projeto:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb+srv://USUARIO:SENHA@cluster.mongodb.net/?appName=NOME_APP",
    "DatabaseName": "CiclismoAPI"
  },
  "Jwt": {
    "SecretKey": "SUA_CHAVE_SECRETA_COM_MINIMO_32_CARACTERES",
    "Issuer": "CiclismoAPI",
    "ExpirationHours": 8
  }
}
```

> ⚠️ Este arquivo está no `.gitignore` e **nunca** deve ser commitado — protege suas credenciais.

### 3. Instale as dependências

```bash
dotnet restore
```

### 4. Execute o projeto

```bash
dotnet run
```

### 5. Acesse a aplicação

| Recurso | URL |
|---------|-----|
| Frontend | http://localhost:5067 |
| Swagger | http://localhost:5067/swagger |
| API Base | http://localhost:5067/api |

> A porta pode variar. Verifique no terminal a mensagem `Now listening on: http://localhost:XXXX`.

---

## 🔐 Variáveis de Ambiente

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `MongoDB:ConnectionString` | String de conexão do MongoDB Atlas | `mongodb+srv://user:pass@cluster.mongodb.net/` |
| `MongoDB:DatabaseName` | Nome do banco de dados | `CiclismoAPI` |
| `Jwt:SecretKey` | Chave para assinar tokens JWT (mín. 32 caracteres) | `MinhaChaveSecreta2026ComMaisDe32Chars` |
| `Jwt:Issuer` | Identificador do emissor do token | `CiclismoAPI` |
| `Jwt:ExpirationHours` | Tempo de expiração do token em horas | `8` |

---

## 📚 Documentação da API

Com o projeto rodando, acesse o Swagger em:

```
http://localhost:5067/swagger
```

O Swagger inclui:
- Descrição de todos os endpoints
- Parâmetros e tipos esperados
- Exemplos de corpo das requisições
- Códigos de resposta documentados (200, 201, 204, 400, 401, 403, 404)
- Botão **Authorize** para testar endpoints protegidos com JWT

**Como usar o botão Authorize:**
1. Faça login em `POST /api/Auth/login`
2. Copie o token retornado
3. Clique em **Authorize** no Swagger
4. Digite: `Bearer SEU_TOKEN_AQUI`
5. Confirme e teste os endpoints protegidos

---

## 📡 Endpoints

### 🔑 Auth — `/api/Auth`

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| `POST` | `/api/Auth/registrar` | Registrar novo usuário | ❌ Público |
| `POST` | `/api/Auth/login` | Login e geração do JWT | ❌ Público |

**Exemplo de registro:**
```json
{
  "nome": "João Silva",
  "email": "joao@email.com",
  "senha": "minhasenha123"
}
```

**Exemplo de login:**
```json
{
  "email": "joao@email.com",
  "senha": "minhasenha123"
}
```

---

### 🚲 Produtos — `/api/Produtos`

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| `GET` | `/api/Produtos` | Listar todos os produtos | ❌ Público |
| `GET` | `/api/Produtos/{id}` | Buscar produto por ID | ❌ Público |
| `POST` | `/api/Produtos` | Criar produto | ✅ Admin |
| `PUT` | `/api/Produtos/{id}` | Atualizar produto completo | ✅ Admin |
| `PATCH` | `/api/Produtos/{id}` | Atualizar campos específicos | ✅ Admin |
| `DELETE` | `/api/Produtos/{id}` | Deletar produto | ✅ Admin |

**Exemplo POST/PUT:**
```json
{
  "nome": "Capacete MTB Pro",
  "descricao": "Capacete para mountain bike com ventilação",
  "categoria": "Segurança",
  "preco": 299.90,
  "estoque": 10
}
```

**Exemplo PATCH (apenas campos desejados):**
```json
{
  "preco": 259.90,
  "estoque": 8
}
```

---

### 📦 Pedidos — `/api/Pedidos`

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| `GET` | `/api/Pedidos` | Listar meus pedidos | ✅ Usuário logado |
| `GET` | `/api/Pedidos/{id}` | Buscar pedido por ID | ✅ Dono do pedido |
| `POST` | `/api/Pedidos` | Criar pedido | ✅ Usuário logado |
| `PATCH` | `/api/Pedidos/{id}` | Atualizar status | ✅ Admin |
| `DELETE` | `/api/Pedidos/{id}` | Deletar pedido | ✅ Dono do pedido |

**Exemplo POST:**
```json
{
  "itens": [
    {
      "produtoId": "6a11f120ef624fa245292edd",
      "quantidade": 2
    }
  ]
}
```

**Exemplo PATCH status:**
```json
{
  "status": "confirmado"
}
```

**Status disponíveis:** `pendente` → `confirmado` → `enviado` → `entregue` / `cancelado`

> ⚠️ Ao cancelar um pedido, o estoque dos produtos é restaurado automaticamente.

> ⚠️ O sistema valida o estoque antes de criar o pedido — pedidos com quantidade maior que o estoque disponível são rejeitados.

---

## 🔑 Autenticação JWT

O sistema usa **JSON Web Tokens (JWT)** com algoritmo HS256.

**Fluxo:**
```
1. POST /api/Auth/login  →  retorna token JWT
2. Incluir no header:  Authorization: Bearer {token}
3. Token expira em 8 horas (configurável em Jwt:ExpirationHours)
```

**Payload do token:**
```json
{
  "nameid": "id_do_usuario",
  "name": "Nome do Usuário",
  "email": "email@email.com",
  "role": "admin",
  "exp": 1735089600,
  "iss": "CiclismoAPI"
}
```

---

## 👥 Perfis de Usuário RBAC

| Perfil | Permissões |
|--------|-----------|
| `cliente` | Ver produtos, criar e visualizar próprios pedidos, gerenciar carrinho |
| `admin` | Tudo do cliente + criar/editar/deletar produtos + atualizar status de pedidos |

> O perfil padrão ao registrar é `cliente`.

> Para promover um usuário a `admin`: MongoDB Atlas → Data Explorer → coleção `usuarios` → editar campo `role` para `"admin"`.

**No frontend:** usuários comuns veem 🔒 nos botões restritos. Administradores veem ✏ e 🗑 em cada produto e têm acesso ao Painel Administrativo completo.

---

## 🌐 Frontend

O frontend é uma **SPA (Single Page Application)** em HTML + JavaScript puro, servida pelo próprio .NET em `http://localhost:5067`.

**Funcionalidades por perfil:**

**Público:**
- Visualizar catálogo de produtos com indicador de estoque
- Criar conta / fazer login

**Usuário logado:**
- Adicionar produtos ao carrinho
- Controlar quantidade e remover itens
- Finalizar pedido com validação de estoque
- Visualizar histórico de pedidos com status

**Admin:**
- Painel Administrativo expansível
- Criar produto — `POST /api/Produtos`
- Editar produto via modal — `PATCH /api/Produtos/{id}`
- Deletar produto — `DELETE /api/Produtos/{id}`
- Gerenciar status de todos os pedidos — `PATCH /api/Pedidos/{id}`

**Navegação assíncrona:** todas as transições entre telas atualizam apenas o conteúdo da página sem recarregar o navegador — padrão SPA com `fetch()`.

---

## 🧪 Testes Unitários

Projeto de testes em `CiclismoAPI.Tests/` usando **xUnit** + **Moq**.

**Para executar:**

```bash
cd CiclismoAPI.Tests
dotnet test
```

**Cenários cobertos:**

| # | Teste | Tipo |
|---|-------|------|
| 1 | Criar produto com dados válidos retorna produto com dados corretos | ✅ Sucesso |
| 2 | Buscar produto por ID existente retorna o produto | ✅ Sucesso |
| 3 | Buscar produto por ID inexistente retorna null | ❌ Erro |
| 4 | Deletar produto inexistente retorna false | ❌ Erro |

**Padrão utilizado:** AAA (Arrange, Act, Assert)

**Por que Moq?** Os testes usam objetos Mock para simular o `IMongoCollection<Produto>` sem conexão real ao banco — garantindo testes rápidos, previsíveis e independentes de rede ou internet.

---

## 🐳 Docker

O projeto inclui `Dockerfile` com **Multi-Stage Build**:

```dockerfile
# Stage 1: compilação com SDK completo
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Stage 2: execução com runtime leve (sem compilador)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
```

**Para construir e executar:**

```bash
docker build -t ciclismoapi .

docker run -p 5067:8080 \
  -e MongoDB__ConnectionString="sua_string_de_conexao" \
  -e MongoDB__DatabaseName="CiclismoAPI" \
  -e Jwt__SecretKey="sua_chave_secreta" \
  ciclismoapi
```

> As credenciais são passadas via variáveis de ambiente — nunca hardcoded na imagem.

---

## 🏛️ Princípios SOLID

Os princípios SOLID aplicados estão documentados em detalhes no arquivo [`Bonus-SOLID.md`](./Bonus-SOLID.md).

**Resumo:**

| Princípio | Onde foi aplicado |
|-----------|------------------|
| **S** — Single Responsibility | Cada Service tem uma única responsabilidade (`ProdutoService`, `PedidoService`, `AuthService`) |
| **O** — Open/Closed | Novos Services são adicionados sem modificar os existentes — basta registrar em `Program.cs` |
| **L** — Liskov Substitution | Todos os Controllers herdam de `ControllerBase` e são tratados uniformemente pelo pipeline ASP.NET |
| **I** — Interface Segregation | DTOs específicos por operação: `ProdutoCriarDTO`, `ProdutoPatchDTO`, `LoginDTO`, `TokenResponseDTO` |
| **D** — Dependency Inversion | Services recebem `IConfiguration` e `IMongoCollection` por injeção — não criam suas próprias dependências |

---

## 📁 Estrutura do Projeto

```
CiclismoAPI/
├── Controllers/
│   ├── AuthController.cs         # Login e registro de usuários
│   ├── ProdutosController.cs     # CRUD completo + PATCH de produtos
│   └── PedidosController.cs      # CRUD de pedidos com proteção IDOR
├── DTOs/
│   ├── AuthDTOs.cs               # RegistrarDTO, LoginDTO, TokenResponseDTO
│   ├── ProdutoDTOs.cs            # CriarDTO, AtualizarDTO, PatchDTO
│   └── PedidoDTOs.cs             # PedidoCriarDTO, ItemPedidoDTO, PedidoPatchDTO
├── Models/
│   ├── Produto.cs                # Entidade com anotações BSON/MongoDB
│   ├── Usuario.cs                # Entidade com SenhaHash e Role
│   └── Pedido.cs                 # Entidade com ItemPedido embutido (NoSQL)
├── Services/
│   ├── ProdutoService.cs         # Regras de negócio + acesso MongoDB
│   ├── PedidoService.cs          # Validação de estoque + atualização automática
│   └── AuthService.cs            # BCrypt + geração de JWT
├── wwwroot/
│   └── index.html                # SPA com painel admin e navegação assíncrona
├── CiclismoAPI.Tests/
│   └── ProdutoServiceTests.cs    # 4 testes xUnit + Moq (AAA pattern)
├── Program.cs                    # Pipeline: MongoDB + JWT + Swagger + RBAC
├── appsettings.json              # Configurações gerais (sem senhas)
├── appsettings.Development.json  # Credenciais locais (no .gitignore)
├── Dockerfile                    # Multi-Stage Build
├── .dockerignore                 # Proteção de arquivos sensíveis na imagem
├── .gitignore                    # Proteção de credenciais no repositório
├── Bonus-SOLID.md                # Documentação dos princípios SOLID
└── README.md                     # Este arquivo
```

---

## 👤 Autor

**Phillip Silva**
Disciplina: Arquitetura de Aplicações Web — 2026.1
Professor: Thalles Noce
Repositório: [github.com/PhillipTI/CiclismoAPI](https://github.com/PhillipTI/CiclismoAPI)

---

## 📅 Prazos

| Evento | Data |
|--------|------|
| Entrega do repositório | 30/05/2026 |
| Apresentação | 01/06 a 05/06/2026 |

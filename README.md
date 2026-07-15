# CineCatalog API 🎬

API REST profissional para gerenciamento de catálogo de filmes, avaliações, favoritos e autenticação de usuários. Desenvolvida em C# com **ASP.NET Core (Web API)** no **.NET 8** seguindo as melhores práticas de desenvolvimento corporativo.

---

## 🏛️ Arquitetura e Decisões Técnicas

O projeto foi construído utilizando os princípios de **Clean Architecture (Arquitetura Limpa)**, mantendo a regra de dependência voltada para o domínio (Core/Domain não possui dependências de infraestrutura ou banco de dados).

### Estrutura de Pastas

```text
CineCatalog API/
├── CineCatalog API/                         # Projeto principal (ASP.NET Core Web API)
│   ├── Domain/                              # Camada de Domínio (Entidades e Interfaces de Contrato)
│   ├── Application/                         # Camada de Aplicação (Serviços, DTOs, Mapeamentos, Validadores)
│   ├── Infrastructure/                      # Camada de Infraestrutura (EF Core, Repositórios, Segurança, Log)
│   ├── Controllers/                         # Apresentação / Endpoints REST
│   └── Extensions/                          # Configurações de DI e Swagger
│
└── CineCatalog.Tests/                       # Projeto de Testes Unitários (xUnit, Moq, FluentAssertions)
```

### Principais Bibliotecas Utilizadas

*   **Entity Framework Core & SQL Server**: ORM padrão para persistência de dados.
*   **JWT Bearer Authentication**: Autenticação stateless baseada em token com suporte a **Refresh Tokens** para renovação segura.
*   **BCrypt.Net-Next**: Hashing seguro de senhas.
*   **FluentValidation**: Validação declarativa de entrada de dados (DTOs) com regras rígidas.
*   **AutoMapper**: Mapeamento seguro e rápido de entidades de domínio para DTOs.
*   **Serilog**: Gravação estruturada de logs no console e arquivos locais gerados diariamente.
*   **xUnit, Moq & FluentAssertions**: Suíte completa de testes unitários automatizados.

---

## 🚀 Funcionalidades

1.  **Usuários e Segurança**:
    *   Cadastro de usuários com regras estritas de senha e verificação de e-mail duplicado.
    *   Autenticação via login e geração de Access Token (JWT) e Refresh Token (armazenado no banco com expiração).
    *   Proteção seletiva de endpoints com base no token JWT.
2.  **Catálogo de Filmes**:
    *   CRUD completo de filmes com relacionamentos complexos.
    *   Busca geral por texto, ordenações e múltiplos filtros (Título, Diretor, Ano, Gênero, Avaliação Mínima, Duração).
    *   Paginação configurável para evitar sobrecarga do servidor.
3.  **Gêneros**:
    *   CRUD e relacionamentos muitos-para-muitos estruturados com filmes.
4.  **Avaliações**:
    *   Usuários logados podem avaliar filmes com nota (1 a 5) e comentário.
    *   Validação que impede um usuário de avaliar o mesmo filme mais de uma vez.
    *   Atualização automática da nota média (`AverageRating`) e quantidade total de avaliações (`ReviewsCount`) do filme após cada avaliação enviada.
5.  **Favoritos**:
    *   Adicionar, remover e listar os filmes favoritos de um usuário.
6.  **Logs e Erros**:
    *   Middleware global para capturar erros e convertê-los no padrão internacional **RFC 7807 (Problem Details)**.

---

## 🛠️ Como Executar Localmente

### Pré-requisitos
*   [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
*   SQL Server (ou LocalDB instalado junto com Visual Studio) ou Docker.

### Passo 1: Clone e Restauração de Dependências
No diretório raiz, execute:
```bash
dotnet restore
```

### Passo 2: Banco de Dados e Migrações
A API está configurada com um script automático de migrações e seed no início da aplicação. Ela tentará se conectar a uma instância de **SQL Server LocalDB** (padrão de instalações do Visual Studio) e criar/popular o banco de dados `CineCatalogDb` automaticamente no primeiro run.

Se precisar rodar manualmente a migração para outro servidor SQL:
1. Altere a string de conexão em `CineCatalog API/appsettings.json`.
2. Execute o comando:
```bash
dotnet ef database update --project "CineCatalog API/CineCatalog API.csproj"
```

### Passo 3: Executar a API
Execute o comando na raiz do projeto:
```bash
dotnet run --project "CineCatalog API/CineCatalog API.csproj"
```

A API iniciará e os logs serão exibidos no console. Acesse o painel do Swagger em:
*   [http://localhost:5024/swagger](http://localhost:5024/swagger) (ou a porta HTTPS configurada na execução)

---

## 🐳 Executando com Docker Compose

Caso possua o Docker instalado, você pode subir o banco de dados SQL Server e a API inteira de forma automatizada com:

```bash
docker-compose up --build
```

Isso criará dois containers:
1.  `cinecatalog-db`: SQL Server rodando na porta `1433`.
2.  `cinecatalog-api`: CineCatalog API exposta nas portas `8080` (HTTP) e `8081` (HTTPS).

---

## 🧪 Executando Testes Automatizados

O projeto contém testes unitários focados nas regras de negócio principais (Autenticação, Avaliações de Filmes e Favoritos).

Para rodar a suíte de testes:
```bash
dotnet test
```

---

## 📚 Como Testar com o Swagger (Passo a Passo)

1.  **Criar Usuário**:
    *   Chame o endpoint `POST /api/Auth/register` enviando um JSON contendo nome, e-mail e senha (ex: `AdminPassword123!`).
2.  **Fazer Login**:
    *   Chame o endpoint `POST /api/Auth/login` com o e-mail e senha criados.
    *   Copie a string recebida no campo `"accessToken"`.
3.  **Autenticar no Swagger**:
    *   No canto superior direito da página do Swagger, clique no botão **Authorize**.
    *   Digite `Bearer ` seguido pelo token copiado (ex: `Bearer eyJhbGciOi...`).
    *   Clique em **Authorize** e feche a janela modal.
4.  **Testar Endpoints Protegidos**:
    *   Agora você poderá chamar endpoints como `POST /api/Movies` (Criar Filme), `POST /api/Movies/{id}/reviews` (Avaliar Filme) e `POST /api/Favorites/{id}` (Favoritar).

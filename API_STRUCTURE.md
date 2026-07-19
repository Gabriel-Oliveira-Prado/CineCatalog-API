# Estrutura de Pastas e Arquivos - CineCatalog API

Este documento descreve detalhadamente a arquitetura de pastas e arquivos da **API** (Backend) do CineCatalog, explicando as responsabilidades de cada componente e camada.

---

## Estrutura da Solução (.slnx)

A solução está estruturada com base na arquitetura em camadas (Clean/Onion Architecture simplificada), separando as regras de domínio, aplicação, infraestrutura e interface de API em diretórios organizados:

* `/CineCatalog API` - Projeto principal da API RESTful (ASP.NET Core 8.0 Web API).
* `/CineCatalog.Tests` - Projeto contendo testes de unidade para os serviços e regras de negócio.
* `CineCatalog API.slnx` - Arquivo de solução moderno do Visual Studio contendo a referência dos projetos.
* `docker-compose.yml` - Arquivo de orquestração para rodar a aplicação em containers Docker.

---

## Detalhamento do Projeto Principal (`CineCatalog API`)

Abaixo estão descritos cada um dos diretórios e arquivos de maior relevância dentro do diretório `/CineCatalog API/CineCatalog API`:

### 1. Camada de Domínio (`/Domain`)
Contém os elementos centrais do domínio de negócio, sem dependências externas a frameworks ou bibliotecas de acesso a dados.
* **/Entities**: As entidades mapeadas para as tabelas do banco de dados:
  * `Movie.cs`: Entidade que representa um filme, guardando metadados (título, diretor, ano, etc.) e o JSON com as plataformas de streaming mapeadas.
  * `Genre.cs`: Gênero cinematográfico (Ação, Comédia, etc.). Tem relação de muitos para muitos com filmes.
  * `User.cs`: Usuário cadastrado no sistema (guarda nome, email, senha hash e o refresh token atual).
  * `Favorite.cs`: Relação que marca se um usuário favoritou um determinado filme.
  * `Review.cs`: Crítica/avaliação escrita por um usuário contendo nota de 1 a 10 e texto.
* **/Interfaces**: Contratos para as implementações das regras de repositório e segurança:
  * `IMovieRepository.cs`, `IGenreRepository.cs`, `IUserRepository.cs`, `IFavoriteRepository.cs`: Definição de assinaturas de acesso ao banco.
  * `IPasswordHasher.cs`, `ITokenService.cs`: Interfaces de criptografia de senhas e geração de tokens JWT.

### 2. Camada de Aplicação (`/Application`)
Implementa os casos de uso, lógica de negócio e regras de validação.
* **/DTOs**: Objetos de transferência de dados usados nas requisições e respostas da API:
  * `AuthDtos.cs`, `GenreDtos.cs`, `MovieDtos.cs`, `ReviewDtos.cs`, `UserProfileDtos.cs`: Estruturas com os campos aceitos de entrada e enviados de saída.
  * `Common/PagedResult.cs`: Container de paginação que embrulha listagens (total de itens, páginas, etc.).
* **/Interfaces**: Contratos para a camada de serviços da aplicação:
  * `IMovieService.cs`, `IGenreService.cs`, `IAuthService.cs`, `IFavoriteService.cs`, `IMovieCatalogSyncService.cs`.
* **/Services**: Implementações concretas das regras de negócio:
  * `MovieService.cs`: CRUD e listagens paginadas de filmes.
  * `GenreService.cs`: Cadastro e listagem de gêneros.
  * `AuthService.cs`: Fluxos de Login, Cadastro, Refresh Token e Alteração de Senha.
  * `FavoriteService.cs`: Gerenciamento de filmes favoritos por usuário.
  * `MovieCatalogSyncService.cs`: Serviço que sincroniza e importa filmes automaticamente a partir do TMDb (The Movie Database).
* **/Validators**: Validações estritas de dados de entrada usando **FluentValidation**:
  * `GenreValidator.cs`, `MovieValidator.cs`, `ReviewValidator.cs`, `UserProfileValidator.cs`, `ChangePasswordValidator.cs`, `RefreshTokenValidator.cs`.
* **/Exceptions**: Exceções customizadas mapeadas para códigos HTTP (ex: `NotFoundException` -> 404, `BadRequestException` -> 400).

### 3. Camada de Infraestrutura (`/Infrastructure`)
Gerencia o acesso a recursos externos (Banco de dados e APIs externas).
* **/Data**: Configuração do Entity Framework Core:
  * `CineCatalogDbContext.cs`: Contexto de acesso ao banco contendo os `DbSet` de todas as tabelas.
  * **Configurations**: Configurações de mapeamento entidade-relacionamento (chaves, índices e relacionamentos muitos-para-muitos).
  * **Seed/DbInitializer.cs**: Aplica migrações pendentes automaticamente ao iniciar o projeto e realiza a limpeza inicial das tabelas se o arquivo `db_cleaned.flag` não existir.
* **/Repositories**: Implementações concretas de acesso ao banco (SQL Server LocalDB / Entity Framework Core) baseadas nos contratos do domínio.
* **/ExternalServices**: Clientes de comunicação externa:
  * **/Tmdb**: Integração com a API do TMDb (`TmdbClient.cs`) para buscar detalhes de filmes em alta, trailer no YouTube e classificação indicativa.
  * **/Security**: Implementações de `PasswordHasher` (usando BCrypt) e `TokenService` (emissão de tokens JWT e Refresh Tokens de segurança).

### 4. Camada de Interface / Controllers (`/Controllers`)
Os pontos de entrada RESTful expostos via HTTP que direcionam para os serviços de aplicação correspondentes.
* `AuthController.cs`: Roteia login, registro, renovação de tokens (refresh) e troca de senha.
* `MoviesController.cs`: Roteia buscas, criação manual de filmes e críticas/reviews associados.
* `GenresController.cs`: Roteia a manipulação de categorias e gêneros cinematográficos.
* `FavoritesController.cs`: Roteia a inclusão e remoção de filmes favoritos do usuário autenticado.
* `UserProfileController.cs`: Roteia a atualização cadastral do perfil do usuário ativo.

### 5. Middlewares (`/Middlewares`)
* `ExceptionHandlingMiddleware.cs`: Intercepta erros não tratados lançados na API e retorna um padrão padronizado em JSON com o respectivo código de status HTTP correto (evita expor erros internos do servidor).

### 6. Configurações Globais e Inicialização
* `Program.cs`: Configuração e montagem do container de injeção de dependências, ativação do Swagger, tratamento de CORS, autenticação JWT, e execução da limpeza/semeadura do banco na inicialização.
* `appsettings.json` / `appsettings.Development.json`: Configurações de conexão de banco (`CineCatalogDb`), credenciais do JWT e caminhos de Logs. As chaves sensíveis do TMDb foram removidas deste arquivo e são salvas usando o **User Secrets** (`secrets.json`) da máquina para evitar exposição no controle de versão.
* `/Properties/launchSettings.json`: Perfis de execução local com a porta de escuta da API REST (`http://localhost:5298`).
# CineCatalog API

API REST do CineCatalog para catálogo de filmes, avaliações, favoritos e autenticação. Desenvolvida em ASP.NET Core 8, com Entity Framework Core e SQL Server.

## Funcionalidades

- Catálogo paginado com busca, filtros e ordenação.
- Cadastro, edição e remoção de avaliações; um usuário só pode avaliar um filme uma vez.
- Favoritos por usuário.
- Status de avaliação do usuário autenticado em cada card (`currentUserReviewRating`).
- Busca complementar no TMDB quando o catálogo local não traz resultados suficientes.
- Consulta de “Onde assistir” no Brasil, separando assinatura, grátis, anúncios, aluguel e compra.

## Onde assistir

`GET /api/Movies/{id}/streaming-platforms` consulta o endpoint atual de provedores do TMDB para a região `BR`. A chamada é feita sob demanda pelo frontend, evitando que a lista de filmes fique mais lenta.

O formato de resposta é padronizado:

```json
{
  "region": "BR",
  "source": "JustWatch via TMDB",
  "link": "https://www.themoviedb.org/movie/.../watch",
  "platforms": [
    {
      "providerId": 8,
      "name": "Netflix",
      "availability": "Assinatura",
      "link": "https://www.themoviedb.org/movie/.../watch"
    }
  ]
}
```

O valor da coluna antiga `StreamingPlatforms` continua sendo lido e é normalizado na saída. Assim, filmes cadastrados antes da mudança não quebram a interface. A disponibilidade vem do JustWatch via TMDB e pode mudar a qualquer momento.

## Arquitetura

```text
CineCatalog API/
├── Application/       # serviços, DTOs, validadores e mapeamentos
├── Controllers/       # endpoints REST
├── Domain/            # entidades e contratos
├── Infrastructure/    # EF Core, repositórios, segurança e cliente TMDB
└── Migrations/        # migrações do banco
```

## Executar localmente

Pré-requisitos: .NET SDK 8 e SQL Server/LocalDB (ou Docker).

```bash
dotnet restore
dotnet run --project "CineCatalog API/CineCatalog API.csproj"
```

O Swagger fica disponível em `http://localhost:5024/swagger` (ou na porta mostrada pelo `dotnet run`).

## Configuração

Defina a conexão e as credenciais do TMDB via `appsettings.*.json`, User Secrets ou variáveis de ambiente:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CineCatalogDb;Trusted_Connection=True"
  },
  "Tmdb": {
    "ApiKey": "sua-chave-ou-token",
    "BaseUrl": "https://api.themoviedb.org/3",
    "ImageBaseUrl": "https://image.tmdb.org/t/p/w500",
    "WatchRegion": "BR"
  }
}
```

O cliente aceita chave v3 na query string ou token de leitura v4 no cabeçalho `Authorization: Bearer`.

## Testes

```bash
dotnet test
```

## Docker

```bash
docker-compose up --build
```

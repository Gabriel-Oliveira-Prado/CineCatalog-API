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

## Configuração da API do TMDb

Este projeto usa a API pública do [TMDb](https://www.themoviedb.org/) para buscar
e importar filmes automaticamente (arquitetura cache-aside: busca primeiro no
banco local, só consulta o TMDb quando necessário).

Para rodar o projeto localmente, você precisa da sua própria chave:

1. Crie uma conta gratuita em https://www.themoviedb.org/signup
2. Acesse https://www.themoviedb.org/settings/api e gere uma API Key (v3) ou
   um Token de Leitura da API (v4)
3. Configure localmente com o .NET User Secrets (nunca commitar a chave):
   ```bash
   dotnet user-secrets set "Tmdb:ApiKey" "sua-chave-aqui"
   ```

Este produto usa a API do TMDb mas não é endossado ou certificado pelo TMDb.


## Testes

```bash
dotnet test
```

## Docker

```bash
docker-compose up --build
```

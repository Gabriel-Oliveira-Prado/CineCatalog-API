using FluentValidation;
using CineCatalog_API.Application.DTOs;
using System;

namespace CineCatalog_API.Application.Validators
{
    public class MovieCreateRequestValidator : AbstractValidator<MovieCreateRequest>
    {
        public MovieCreateRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("O título do filme é obrigatório.")
                .MaximumLength(200).WithMessage("O título não pode exceder 200 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("A descrição não pode exceder 1000 caracteres.");

            RuleFor(x => x.Synopsis)
                .NotEmpty().WithMessage("A sinopse é obrigatória.")
                .MaximumLength(4000).WithMessage("A sinopse não pode exceder 4000 caracteres.");

            RuleFor(x => x.Director)
                .NotEmpty().WithMessage("O diretor é obrigatório.")
                .MaximumLength(150).WithMessage("O nome do diretor não pode exceder 150 caracteres.");

            RuleFor(x => x.Cast)
                .NotEmpty().WithMessage("O elenco é obrigatório.")
                .MaximumLength(2000).WithMessage("O elenco não pode exceder 2000 caracteres.");

            RuleFor(x => x.ReleaseYear)
                .InclusiveBetween(1888, DateTime.Now.Year + 5)
                .WithMessage($"O ano de lançamento deve estar entre 1888 e {DateTime.Now.Year + 5}.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).WithMessage("A duração do filme deve ser maior que 0 minutos.");

            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage("A classificação indicativa é obrigatória.")
                .MaximumLength(20).WithMessage("A classificação indicativa não pode exceder 20 caracteres.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("A URL do pôster é obrigatória.")
                .MaximumLength(500).WithMessage("A URL do pôster não pode exceder 500 caracteres.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
                .WithMessage("A URL do pôster deve ser um link HTTP/HTTPS válido.");

            RuleFor(x => x.TrailerUrl)
                .MaximumLength(500).WithMessage("A URL do trailer não pode exceder 500 caracteres.")
                .Must(uri => string.IsNullOrEmpty(uri) || (Uri.TryCreate(uri, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps)))
                .WithMessage("A URL do trailer deve ser um link HTTP/HTTPS válido.");

            RuleFor(x => x.StreamingPlatforms)
                .Must(json => 
                {
                    if (string.IsNullOrEmpty(json)) return true;
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(json);
                        return doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .WithMessage("As plataformas de streaming devem ser um array JSON válido.");

            RuleFor(x => x.GenreIds)
                .NotEmpty().WithMessage("O filme deve conter pelo menos um gênero cadastrado.");
        }
    }

    public class MovieUpdateRequestValidator : AbstractValidator<MovieUpdateRequest>
    {
        public MovieUpdateRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("O título do filme é obrigatório.")
                .MaximumLength(200).WithMessage("O título não pode exceder 200 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("A descrição não pode exceder 1000 caracteres.");

            RuleFor(x => x.Synopsis)
                .NotEmpty().WithMessage("A sinopse é obrigatória.")
                .MaximumLength(4000).WithMessage("A sinopse não pode exceder 4000 caracteres.");

            RuleFor(x => x.Director)
                .NotEmpty().WithMessage("O diretor é obrigatório.")
                .MaximumLength(150).WithMessage("O nome do diretor não pode exceder 150 caracteres.");

            RuleFor(x => x.Cast)
                .NotEmpty().WithMessage("O elenco é obrigatório.")
                .MaximumLength(2000).WithMessage("O elenco não pode exceder 2000 caracteres.");

            RuleFor(x => x.ReleaseYear)
                .InclusiveBetween(1888, DateTime.Now.Year + 5)
                .WithMessage($"O ano de lançamento deve estar entre 1888 e {DateTime.Now.Year + 5}.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).WithMessage("A duração do filme deve ser maior que 0 minutos.");

            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage("A classificação indicativa é obrigatória.")
                .MaximumLength(20).WithMessage("A classificação indicativa não pode exceder 20 caracteres.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("A URL do pôster é obrigatória.")
                .MaximumLength(500).WithMessage("A URL do pôster não pode exceder 500 caracteres.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
                .WithMessage("A URL do pôster deve ser um link HTTP/HTTPS válido.");

            RuleFor(x => x.TrailerUrl)
                .MaximumLength(500).WithMessage("A URL do trailer não pode exceder 500 caracteres.")
                .Must(uri => string.IsNullOrEmpty(uri) || (Uri.TryCreate(uri, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps)))
                .WithMessage("A URL do trailer deve ser um link HTTP/HTTPS válido.");

            RuleFor(x => x.StreamingPlatforms)
                .Must(json => 
                {
                    if (string.IsNullOrEmpty(json)) return true;
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(json);
                        return doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .WithMessage("As plataformas de streaming devem ser um array JSON válido.");

            RuleFor(x => x.GenreIds)
                .NotEmpty().WithMessage("O filme deve conter pelo menos um gênero cadastrado.");
        }
    }
}
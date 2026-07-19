using FluentValidation;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Validators
{
    public class UserUpdateProfileRequestValidator : AbstractValidator<UserUpdateProfileRequest>
    {
        public UserUpdateProfileRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(60).WithMessage("O nome não pode exceder 60 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("Formato de e-mail inválido.")
                .MaximumLength(256).WithMessage("O e-mail não pode exceder 256 caracteres.");

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(500).WithMessage("A URL do avatar não pode exceder 500 caracteres.")
                .Must(uri => string.IsNullOrEmpty(uri) || (Uri.TryCreate(uri, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps)))
                .WithMessage("A URL do avatar deve ser um link HTTP/HTTPS válido.");
        }
    }
}
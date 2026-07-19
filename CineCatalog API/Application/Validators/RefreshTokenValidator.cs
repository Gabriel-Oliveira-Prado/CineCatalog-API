using FluentValidation;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Validators
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("O token de acesso é obrigatório.");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("O token de atualização (refresh token) é obrigatório.");
        }
    }
}
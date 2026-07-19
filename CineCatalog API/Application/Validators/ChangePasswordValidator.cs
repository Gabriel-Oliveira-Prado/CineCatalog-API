using FluentValidation;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Validators
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Senha atual é obrigatória.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Nova senha é obrigatória.")
                .MinimumLength(8).WithMessage("A nova senha deve conter no mínimo 8 caracteres.")
                .Matches("[A-Z]").WithMessage("A nova senha deve conter pelo menos uma letra maiúscula.")
                .Matches("[a-z]").WithMessage("A nova senha deve conter pelo menos uma letra minúscula.")
                .Matches("[0-9]").WithMessage("A nova senha deve conter pelo menos um número.")
                .Matches("[^a-zA-Z0-9]").WithMessage("A nova senha deve conter pelo menos um caractere especial.");
        }
    }
}
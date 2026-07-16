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
        }
    }
}

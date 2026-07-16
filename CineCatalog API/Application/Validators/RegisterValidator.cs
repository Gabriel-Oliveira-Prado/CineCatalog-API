using FluentValidation;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Validators
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
        {
            public UserLoginRequestValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("E-mail é obrigatório.")
                    .EmailAddress().WithMessage("E-mail em formato inválido.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Senha é obrigatória.");
            }
        }

        public UserRegisterRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .Length(3, 60).WithMessage("O nome deve ter entre 3 e 60 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório.")
                .EmailAddress().WithMessage("E-mail em formato inválido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória.")
                .MinimumLength(8).WithMessage("A senha deve conter no mínimo 8 caracteres.")
                .Matches("[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
                .Matches("[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
                .Matches("[0-9]").WithMessage("A senha deve conter pelo menos um número.")
                .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter pelo menos um caractere especial.");
        }
    }

    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório.")
                .EmailAddress().WithMessage("E-mail em formato inválido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatória.");
        }
    }
}

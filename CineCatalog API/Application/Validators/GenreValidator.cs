using FluentValidation;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Validators
{
    public class GenreCreateRequestValidator : AbstractValidator<GenreCreateRequest>
    {
        public GenreCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do gênero é obrigatório.")
                .Length(3, 50).WithMessage("O nome do gênero deve ter entre 3 e 50 caracteres.");
        }
    }

    public class GenreUpdateRequestValidator : AbstractValidator<GenreUpdateRequest>
    {
        public GenreUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do gênero é obrigatório.")
                .Length(3, 50).WithMessage("O nome do gênero deve ter entre 3 e 50 caracteres.");
        }
    }
}

using FluentValidation;
using CineCatalog_API.Application.DTOs;

namespace CineCatalog_API.Application.Validators
{
    public class ReviewCreateRequestValidator : AbstractValidator<ReviewCreateRequest>
    {
        public ReviewCreateRequestValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("A avaliação deve ser uma nota de 1 a 5.");

            RuleFor(x => x.Comment)
                .Must(c => string.IsNullOrEmpty(c) || !string.IsNullOrWhiteSpace(c))
                .WithMessage("O comentário não pode consistir apenas de espaços em branco.")
                .MaximumLength(2000).WithMessage("O comentário não pode exceder 2000 caracteres.");
        }
    }

    public class ReviewUpdateRequestValidator : AbstractValidator<ReviewUpdateRequest>
    {
        public ReviewUpdateRequestValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("A avaliação deve ser uma nota de 1 a 5.");

            RuleFor(x => x.Comment)
                .Must(c => string.IsNullOrEmpty(c) || !string.IsNullOrWhiteSpace(c))
                .WithMessage("O comentário não pode consistir apenas de espaços em branco.")
                .MaximumLength(2000).WithMessage("O comentário não pode exceder 2000 caracteres.");
        }
    }
}

using FluentValidation;

namespace Application.Commands.AuthorCommands.Add
{
    public class AddAuthorCommandValidator : AbstractValidator<AddAuthorCommand>
    {
        public AddAuthorCommandValidator()
        {
            RuleFor(x => x.NewAuthor.FirstName)
                .NotNull().NotEmpty().WithMessage("Name is required.")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");

            RuleFor(x => x.NewAuthor.LastName)
                .NotNull().WithMessage("Name is required.")
                .NotEmpty().WithMessage("Lastname is required.")
                .MinimumLength(3).WithMessage("Password must be at least 3 characters long.");
        }
    }
}

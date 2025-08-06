using FluentValidation;
using DTOs.Auth;

namespace DTOs.Validators;

public class ConfirmEmailRequestDtoValidator : AbstractValidator<ConfirmEmailRequestDto>
{
    public ConfirmEmailRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please provide a valid email address.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Confirmation token is required.")
            .MaximumLength(512).WithMessage("Invalid confirmation token format.");
    }
}

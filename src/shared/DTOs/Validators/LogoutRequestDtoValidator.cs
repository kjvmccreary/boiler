using FluentValidation;
using DTOs.Auth;

namespace DTOs.Validators;

public class LogoutRequestDtoValidator : AbstractValidator<LogoutRequestDto>
{
    public LogoutRequestDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MinimumLength(32).WithMessage("Invalid refresh token format.")
            .MaximumLength(512).WithMessage("Invalid refresh token format.");
    }
}

using FluentValidation;
using DTOs.Auth;

namespace DTOs.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please provide a valid email address.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("Password and confirmation password do not match.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.");

        // Optional tenant fields validation
        RuleFor(x => x.TenantName)
            .MaximumLength(255).WithMessage("Tenant name cannot exceed 255 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$").WithMessage("Tenant name can only contain letters, numbers, spaces, hyphens, and underscores.")
            .When(x => !string.IsNullOrEmpty(x.TenantName));

        RuleFor(x => x.TenantDomain)
            .MaximumLength(255).WithMessage("Tenant domain cannot exceed 255 characters.")
            .Matches(@"^[a-zA-Z0-9\-\.]+$").WithMessage("Tenant domain can only contain letters, numbers, hyphens, and dots.")
            .When(x => !string.IsNullOrEmpty(x.TenantDomain));
    }
}

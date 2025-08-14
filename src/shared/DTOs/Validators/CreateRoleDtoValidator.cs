using FluentValidation;
using DTOs.Auth; // ← CHANGE: from DTOs.Role to DTOs.Auth

namespace DTOs.Validators;

public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required")
            .Length(1, 100).WithMessage("Role name must be between 1 and 100 characters")
            .Matches("^[a-zA-Z0-9_\\-\\s]+$").WithMessage("Role name contains invalid characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .Must(desc => !desc?.Contains("<script>") == true).WithMessage("Description contains potentially unsafe content");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions list cannot be null");
    }
}

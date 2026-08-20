using FluentValidation;

namespace SupportTicketing.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request.FullName).NotEmpty().WithMessage("Full Name is required.").MaximumLength(50).WithMessage("Full Name can not exceed 50 char.");

        RuleFor(x => x.Request.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Request.Password).NotEmpty().WithMessage("Password is required.").MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        RuleFor(x => x.Request.ConfirmPassword).Equal(x => x.Request.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.Request.Role).NotEmpty().WithMessage("Role is required.");;
    }
}
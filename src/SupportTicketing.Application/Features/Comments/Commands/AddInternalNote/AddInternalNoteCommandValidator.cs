using FluentValidation;

namespace SupportTicketing.Application.Features.Comments.Commands.AddInternalNote;

public class AddInternalNoteCommandValidator : AbstractValidator<AddInternalNoteCommand>
{
    public AddInternalNoteCommandValidator()
    {
        RuleFor(x => x.Request.Content)
            .NotEmpty()
            .WithMessage("Internal note content is required.");

        RuleFor(x => x.Request.Content)
            .MaximumLength(4000)
            .WithMessage("Internal note cannot exceed 4000 characters.");
    }
}
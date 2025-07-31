using FluentValidation;
using TurnAroundPromptApi.Services.Models;

namespace TurnAroundPromptApi.Api.Validators
{
    public class TurnAroundPromptValidator : AbstractValidator<TurnAroundPrompt>
    {
        public TurnAroundPromptValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .Matches(@"^TAP-\d+$")
                .WithMessage("ID must match pattern TAP-{number}");

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(255)
                .WithMessage("Name must be between 1 and 255 characters");

            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(BeValidStatus)
                .WithMessage("Status must be one of: active, inactive, pending, completed");
        }

        private bool BeValidStatus(string status)
        {
            return Enum.TryParse<TurnAroundPromptStatus>(status, true, out _);
        }
    }
} 
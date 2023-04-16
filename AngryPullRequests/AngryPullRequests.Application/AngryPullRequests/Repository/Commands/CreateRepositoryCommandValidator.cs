using FluentValidation;

namespace AngryPullRequests.Application.AngryPullRequests.Commands
{
    public class CreateRepositoryCommandValidator : AbstractValidator<CreateRepositoryCommand>
    {
        public CreateRepositoryCommandValidator() { }
    }
}

using AdvancedFullProject.Application.Employees.Commands;
using FluentValidation;

namespace AdvancedFullProject.Application.Employees.Validators;

public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Department).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Salary).GreaterThan(0);
    }
}

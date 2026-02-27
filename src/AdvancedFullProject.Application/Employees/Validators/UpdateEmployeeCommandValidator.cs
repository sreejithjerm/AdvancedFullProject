using AdvancedFullProject.Application.Employees.Commands;
using FluentValidation;

namespace AdvancedFullProject.Application.Employees.Validators;

public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        Include(new CreateEmployeeCommandValidator());
        RuleFor(x => x.Id).NotEmpty();
    }
}

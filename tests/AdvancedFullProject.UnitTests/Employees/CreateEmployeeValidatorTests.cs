using AdvancedFullProject.Application.Employees.Commands;
using AdvancedFullProject.Application.Employees.Validators;
using FluentAssertions;

namespace AdvancedFullProject.UnitTests.Employees;

public sealed class CreateEmployeeValidatorTests
{
    [Fact]
    public void Should_Fail_When_Email_Is_Invalid()
    {
        var validator = new CreateEmployeeCommandValidator();
        var command = new CreateEmployeeCommand("John", "Doe", "invalid-email", "IT", 5000);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}

using EmployeeManagement.Application.DTOs;
using FluentValidation;

namespace EmployeeManagement.Application.Validators;

public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile number is required")
            .MaximumLength(20).WithMessage("Mobile number cannot exceed 20 characters");

        RuleFor(x => x.JobTitle)
            .NotEmpty().WithMessage("Job title is required")
            .MaximumLength(100).WithMessage("Job title cannot exceed 100 characters");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("Hire date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Hire date cannot be in the future");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("A valid department must be selected");
    }
}

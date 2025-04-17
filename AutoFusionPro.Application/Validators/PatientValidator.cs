using AutoFusionPro.Domain.Models;
using FluentValidation;

namespace AutoFusionPro.Application.Validators
{
    //public class PatientValidator : AbstractValidator<Patient>
    //{
    //    //public PatientValidator()
    //    //{
    //    //    RuleFor(p => p.Name)
    //    //      .NotEmpty().WithMessage("Name is required")
    //    //      .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");


    //    //    RuleFor(p => p.Age)
    //    //        .NotEmpty().WithMessage("Age is required")
    //    //        .LessThan(0).WithMessage("Age cannot be 0")
    //    //        .GreaterThan(120).WithMessage("Age cannot be more than 120");

    //    //    RuleFor(p => p.PhoneNumber)
    //    //        .NotEmpty().WithMessage("Phone number is required")
    //    //        .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
    //    //        .Matches(@"^[0-9\+\-\(\) ]+$").WithMessage("Phone number contains invalid characters");

    //    //    RuleFor(p => p.AlternatePhoneNumber)
    //    //        .MaximumLength(20).WithMessage("Alternate phone number cannot exceed 20 characters")
    //    //        .Matches(@"^[0-9\+\-\(\) ]+$").WithMessage("Alternate phone number contains invalid characters")
    //    //        .When(p => !string.IsNullOrEmpty(p.AlternatePhoneNumber));

    //    //    RuleFor(p => p.Email)
    //    //        .EmailAddress().WithMessage("Invalid email address format")
    //    //        .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
    //    //        .When(p => !string.IsNullOrEmpty(p.Email));

    //    //    RuleFor(p => p.Address)
    //    //        .MaximumLength(255).WithMessage("Address cannot exceed 255 characters");

    //    //    RuleFor(p => p.EmergencyContactName)
    //    //        .MaximumLength(100).WithMessage("Emergency contact name cannot exceed 100 characters");

    //    //    RuleFor(p => p.EmergencyContactPhoneNumber)
    //    //        .MaximumLength(20).WithMessage("Emergency contact phone number cannot exceed 20 characters")
    //    //        .Matches(@"^[0-9\+\-\(\) ]+$").WithMessage("Emergency contact phone number contains invalid characters")
    //    //        .When(p => !string.IsNullOrEmpty(p.EmergencyContactPhoneNumber));

    //    //    RuleFor(p => p.BloodType)
    //    //        .MaximumLength(10).WithMessage("Blood type cannot exceed 10 characters");

    //    //    RuleFor(p => p.AllergiesNotes)
    //    //        .MaximumLength(500).WithMessage("Allergies notes cannot exceed 500 characters");

    //    //    RuleFor(p => p.MedicalHistoryNotes)
    //    //        .MaximumLength(1000).WithMessage("Medical history notes cannot exceed 1000 characters");

    //    //    RuleFor(p => p.Notes)
    //    //        .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
    //    //}
    //}
}

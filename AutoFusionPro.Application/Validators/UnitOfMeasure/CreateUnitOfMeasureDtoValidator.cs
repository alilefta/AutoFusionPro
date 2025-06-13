using AutoFusionPro.Application.DTOs.UnitOfMeasure;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Application.Validators.UnitOfMeasure
{
    public class CreateUnitOfMeasureDtoValidator : AbstractValidator<CreateUnitOfMeasureDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateUnitOfMeasureDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Unit of Measure name is required.")
                .MaximumLength(50).WithMessage("Unit of Measure name cannot exceed 50 characters.")
                .MustAsync(BeUniqueName).WithMessage(dto => $"The Unit of Measure name '{dto.Name}' already exists.");

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Unit of Measure symbol is required.")
                .MaximumLength(10).WithMessage("Unit of Measure symbol cannot exceed 10 characters.")
                .MustAsync(BeUniqueSymbol).WithMessage(dto => $"The Unit of Measure symbol '{dto.Symbol}' already exists.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Description cannot exceed 255 characters.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name)) return true; // Handled by NotEmpty
            return !await _unitOfWork.UnitOfMeasures.NameExistsAsync(name);
        }

        private async Task<bool> BeUniqueSymbol(string symbol, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(symbol)) return true; // Handled by NotEmpty
            return !await _unitOfWork.UnitOfMeasures.SymbolExistsAsync(symbol);
        }
    }
}

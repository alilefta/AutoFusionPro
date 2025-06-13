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
    public class UpdateUnitOfMeasureDtoValidator : AbstractValidator<UpdateUnitOfMeasureDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUnitOfMeasureDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid Unit of Measure ID.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Unit of Measure name is required.")
                .MaximumLength(50).WithMessage("Unit of Measure name cannot exceed 50 characters.")
                .MustAsync(async (dto, name, context, token) => await BeUniqueNameExcludingSelf(dto.Id, name, token))
                .WithMessage(dto => $"The Unit of Measure name '{dto.Name}' already exists.");

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Unit of Measure symbol is required.")
                .MaximumLength(10).WithMessage("Unit of Measure symbol cannot exceed 10 characters.")
                .MustAsync(async (dto, symbol, context, token) => await BeUniqueSymbolExcludingSelf(dto.Id, symbol, token))
                .WithMessage(dto => $"The Unit of Measure symbol '{dto.Symbol}' already exists.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Description cannot exceed 255 characters.");
        }

        private async Task<bool> BeUniqueNameExcludingSelf(int id, string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name)) return true; // Handled by NotEmpty
            return !await _unitOfWork.UnitOfMeasures.NameExistsAsync(name, id);
        }

        private async Task<bool> BeUniqueSymbolExcludingSelf(int id, string symbol, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(symbol)) return true; // Handled by NotEmpty
            return !await _unitOfWork.UnitOfMeasures.SymbolExistsAsync(symbol, id);
        }
    }
}

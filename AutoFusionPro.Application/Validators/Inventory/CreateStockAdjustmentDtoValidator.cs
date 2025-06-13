using AutoFusionPro.Application.DTOs.InventoryTransactions;
using AutoFusionPro.Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Application.Validators.Inventory
{
    public class CreateStockAdjustmentDtoValidator : AbstractValidator<CreateStockAdjustmentDto>
    {
        public CreateStockAdjustmentDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.PartId)
                .GreaterThan(0).WithMessage("Part ID is required.");
            // Optionally check if PartId exists
            // .MustAsync(async (id, token) => await unitOfWork.Parts.ExistsAsync(p => p.Id == id))
            // .WithMessage("Selected Part does not exist.");

            RuleFor(x => x.NewActualQuantityOnHand)
                .GreaterThanOrEqualTo(0).WithMessage("New quantity on hand must be zero or positive.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("A reason for the adjustment is required.")
                .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID is required.");
            // Optionally check if UserId exists
            // .MustAsync(async (id, token) => await unitOfWork.Users.ExistsAsync(u => u.Id == id))
            // .WithMessage("Performing User does not exist.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
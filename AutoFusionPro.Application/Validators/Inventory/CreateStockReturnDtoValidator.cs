using AutoFusionPro.Application.DTOs.InventoryTransactions;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Application.Validators.Inventory
{
    public class CreateStockReturnDtoValidator : AbstractValidator<CreateStockReturnDto>
    {
        public CreateStockReturnDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.PartId)
                .GreaterThan(0).WithMessage("Part ID is required.");
            // .MustAsync(async (id, token) => await unitOfWork.Parts.ExistsAsync(p => p.Id == id))
            // .WithMessage("Selected Part does not exist.");

            RuleFor(x => x.QuantityReturned)
                .GreaterThan(0).WithMessage("Quantity returned must be positive.");

            RuleFor(x => x.ReturnType)
                .IsInEnum().WithMessage("A valid return type must be specified.");

            RuleFor(x => x.Reference)
                .NotEmpty().WithMessage("A reference (e.g., RMA Number, Original SO/PO) is required.")
                .MaximumLength(50).WithMessage("Reference cannot exceed 50 characters.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID is required.");
            // .MustAsync(async (id, token) => await unitOfWork.Users.ExistsAsync(u => u.Id == id))
            // .WithMessage("Performing User does not exist.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");

            // Optional: Validate OriginalSalesOrderId or OriginalPurchaseOrderId exist if provided
            When(x => x.OriginalSalesOrderId.HasValue && x.OriginalSalesOrderId.Value > 0, () => {
                RuleFor(x => x.OriginalSalesOrderId)
                    .MustAsync(async (id, token) => await unitOfWork.ExistsAsync<Order>(o => o.Id == id!.Value)) // Assuming Order entity & repo
                    .WithMessage("Original Sales Order ID does not exist.");
            });
            // Similar for OriginalPurchaseOrderId if you have PurchaseOrder entity
        }
    }
}

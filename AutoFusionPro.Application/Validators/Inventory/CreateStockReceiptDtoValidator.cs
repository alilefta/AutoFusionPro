using AutoFusionPro.Application.DTOs.InventoryTransactions;
using AutoFusionPro.Domain.Interfaces;
using AutoFusionPro.Domain.Models;
using FluentValidation;

namespace AutoFusionPro.Application.Validators.Inventory
{
    public class CreateStockReceiptDtoValidator : AbstractValidator<CreateStockReceiptDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateStockReceiptDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.PartId)
                .GreaterThan(0).WithMessage("Part ID must be greater than zero.")
                .MustAsync(async (id, token) => await _unitOfWork.Parts.ExistsAsync(p => p.Id == id))
                .WithMessage(x => $"Part with ID '{x.PartId}' does not exist.");

            RuleFor(x => x.QuantityReceived)
                .GreaterThan(0).WithMessage("Quantity received must be a positive number.");

            RuleFor(x => x.Reference)
                .NotEmpty().WithMessage("A reference (e.g., PO Number, 'Initial Stock') is required.")
                .MaximumLength(50).WithMessage("Reference cannot exceed 50 characters.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID is required.")
                .MustAsync(async (id, token) => await _unitOfWork.Users.ExistsAsync(u => u.Id == id)) // Assumes IUserRepository on UoW
                .WithMessage(x => $"User with ID '{x.UserId}' does not exist.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");

            // Optional: Validate Purchase if provided
            When(x => x.PurchaseOrderId.HasValue && x.PurchaseOrderId.Value > 0, () =>
            {
                RuleFor(x => x.PurchaseOrderId)
                    .MustAsync(async (id, token) =>
                        // Assuming you have a Purchase entity and IUnitOfWork.Purchases.ExistsAsync
                        // For now, let's stub it. Replace with actual check later.
                        // This check might be simpler if Purchase is a dedicated repo on IUnitOfWork.
                        // For now, using generic ExistsAsync as an example.
                        await _unitOfWork.ExistsAsync<Purchase>(po => po.Id == id!.Value) // Replace PurchaseOrder with your actual entity
                    )
                    .WithMessage(x => $"Purchase Order with ID '{x.PurchaseOrderId}' does not exist.");
            });
        }
    }
}

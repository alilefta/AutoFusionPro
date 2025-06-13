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
    public class CreateStockDispatchDtoValidator : AbstractValidator<CreateStockDispatchDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateStockDispatchDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            RuleFor(x => x.PartId)
                .GreaterThan(0).WithMessage("Part ID must be greater than zero.")
                .MustAsync(async (id, token) => await _unitOfWork.Parts.ExistsAsync(p => p.Id == id))
                .WithMessage(x => $"Part with ID '{x.PartId}' does not exist.");

            RuleFor(x => x.QuantityDispatched)
                .GreaterThan(0).WithMessage("Quantity dispatched must be a positive number.");

            RuleFor(x => x.Reference)
                .NotEmpty().WithMessage("A reference (e.g., Sales Order Number, Internal Requisition) is required.")
                .MaximumLength(50).WithMessage("Reference cannot exceed 50 characters.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID is required.")
                .MustAsync(async (id, token) => await _unitOfWork.Users.ExistsAsync(u => u.Id == id))
                .WithMessage(x => $"User with ID '{x.UserId}' does not exist.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");

            // Optional: Validate SalesOrderId if provided
            When(x => x.SalesOrderId.HasValue && x.SalesOrderId.Value > 0, () =>
            {
                RuleFor(x => x.SalesOrderId)
                    .MustAsync(async (id, token) =>
                        await _unitOfWork.ExistsAsync<Order>(o => o.Id == id!.Value) // Assuming 'Order' is your Sales Order entity
                    )
                    .WithMessage(x => $"Sales Order with ID '{x.SalesOrderId}' does not exist.");
            });

            // Optional: Validate InvoiceId if provided
            When(x => x.InvoiceId.HasValue && x.InvoiceId.Value > 0, () =>
            {
                RuleFor(x => x.InvoiceId)
                    .MustAsync(async (id, token) =>
                        await _unitOfWork.ExistsAsync<Invoice>(i => i.Id == id!.Value) // Assuming 'Invoice' is your entity
                    )
                    .WithMessage(x => $"Invoice with ID '{x.InvoiceId}' does not exist.");
            });
        }
    }
}

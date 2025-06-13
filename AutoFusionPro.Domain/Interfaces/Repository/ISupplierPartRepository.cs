using AutoFusionPro.Domain.Interfaces.Repository.Base;
using AutoFusionPro.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Domain.Interfaces.Repository
{
    public interface ISupplierPartRepository : IBaseRepository<SupplierPart>
    {
        Task<IEnumerable<SupplierPart>> GetByPartIdWithSupplierDetailsAsync(int partId);

        /// <summary>
        /// Gets all preferred SupplierPart links for a specific part,
        /// optionally excluding one specific SupplierPart link.
        /// </summary>
        /// <param name="partId">The ID of the part.</param>
        /// <param name="excludeSupplierPartId">Optional ID of a SupplierPart link to exclude from the results.</param>
        /// <returns>A collection of preferred SupplierPart entities.</returns>
        Task<IEnumerable<SupplierPart>> GetPreferredSupplierLinksForPartAsync(int partId, int? excludeSupplierPartId = null);

    }
}

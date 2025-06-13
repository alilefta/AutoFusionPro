using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class SupplierRepository : Repository<Supplier, SupplierRepository>, ISupplierRepository
    {

        #region Constructor
        /// <summary>
        /// Default Constructor for <see cref="SupplierRepository"/>
        /// Context and Logger are provided by <see cref="Repository{T, Repo}"/>
        /// </summary>
        /// <param name="context"><see cref="ApplicationDbContext"/></param>
        /// <param name="logger"><see cref="ILogger"/></param>
        public SupplierRepository(ApplicationDbContext context, ILogger<SupplierRepository> logger) : base(context, logger) { }
        #endregion
    }
}

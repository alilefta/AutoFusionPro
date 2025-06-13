using AutoFusionPro.Core.Exceptions.Repository;
using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class UnitOfMeasureRepository : Repository<UnitOfMeasure, UnitOfMeasureRepository>, IUnitOfMeasureRepository
    {
        public UnitOfMeasureRepository(ApplicationDbContext context, ILogger<UnitOfMeasureRepository> logger) : base(context, logger)
        {
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeUnitOfMeasureId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Attempted to check name existence with null or empty name.");
                return false; // Or throw ArgumentException
            }

            string nameLower = name.Trim().ToLowerInvariant();
            _logger.LogDebug("Checking name existence for: Name='{Name}', ExcludeId='{ExcludeId}'",
                nameLower, excludeUnitOfMeasureId);
            try
            {
                var query = _dbSet.Where(c => c.Name.ToLower() == nameLower);

                if (excludeUnitOfMeasureId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeUnitOfMeasureId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking unit of measure name existence for Name '{Name}'.", name);
                throw new RepositoryException($"Could not check existence for unit of measure's name '{name}'.", ex);
            }
        }

        public async Task<bool> SymbolExistsAsync(string symbol, int? excludeUnitOfMeasureId = null)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                _logger.LogWarning("Attempted to check symbol existence with null or empty symbol.");
                return false; // Or throw ArgumentException
            }

            string symbolLower = symbol.Trim().ToLowerInvariant();
            _logger.LogDebug("Checking symbol existence for: Symbol='{Symbol}', ExcludeId='{ExcludeId}'",
                symbolLower, excludeUnitOfMeasureId);
            try
            {
                var query = _dbSet.Where(c => c.Symbol.ToLower() == symbolLower);

                if (excludeUnitOfMeasureId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeUnitOfMeasureId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking unit of measure symbol existence for Symbol '{Name}'.", symbol);
                throw new RepositoryException($"Could not check existence for unit of measure's Symbol '{symbol}'.", ex);
            }
        }
    }
}

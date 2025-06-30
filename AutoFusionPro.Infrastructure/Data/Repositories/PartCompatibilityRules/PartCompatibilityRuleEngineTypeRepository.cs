using AutoFusionPro.Domain.Interfaces.Repository.PartCompatibilityRulesRepositories;
using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFusionPro.Infrastructure.Data.Repositories.PartCompatibilityRules
{
    public class PartCompatibilityRuleEngineTypeRepository : Repository<PartCompatibilityRuleEngineType, PartCompatibilityRuleEngineTypeRepository>, IPartCompatibilityRuleEngineTypeRepository
    {
        public PartCompatibilityRuleEngineTypeRepository(ApplicationDbContext context, ILogger<PartCompatibilityRuleEngineTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

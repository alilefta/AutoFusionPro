using AutoFusionPro.Domain.Interfaces.Repository.PartCompatibilityRulesRepositories;
using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.PartCompatibilityRules
{
    public class PartCompatibilityRuleTransmissionTypeRepository : Repository<PartCompatibilityRuleTransmissionType, PartCompatibilityRuleTransmissionTypeRepository>, IPartCompatibilityRuleTransmissionTypeRepository
    {
        public PartCompatibilityRuleTransmissionTypeRepository(ApplicationDbContext context, ILogger<PartCompatibilityRuleTransmissionTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

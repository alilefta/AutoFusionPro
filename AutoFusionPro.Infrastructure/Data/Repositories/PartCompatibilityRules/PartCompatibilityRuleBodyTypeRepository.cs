using AutoFusionPro.Domain.Interfaces.Repository.PartCompatibilityRulesRepositories;
using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.PartCompatibilityRules
{
    public class PartCompatibilityRuleBodyTypeRepository : Repository<PartCompatibilityRuleBodyType, PartCompatibilityRuleBodyTypeRepository>, IPartCompatibilityRuleBodyTypeRepository
    {
        public PartCompatibilityRuleBodyTypeRepository(ApplicationDbContext context, ILogger<PartCompatibilityRuleBodyTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

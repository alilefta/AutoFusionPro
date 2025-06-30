using AutoFusionPro.Domain.Interfaces.Repository.PartCompatibilityRulesRepositories;
using AutoFusionPro.Domain.Models.PartCompatibilityRules;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories.PartCompatibilityRules
{
    public class PartCompatibilityRuleTrimLevelRepository : Repository<PartCompatibilityRuleTrimLevel, PartCompatibilityRuleTrimLevelRepository>, IPartCompatibilityRuleTrimLevelRepository
    {
        public PartCompatibilityRuleTrimLevelRepository(ApplicationDbContext context, ILogger<PartCompatibilityRuleTrimLevelRepository> logger) : base(context, logger)
        {
        }
    }
}

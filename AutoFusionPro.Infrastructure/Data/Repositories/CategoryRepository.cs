using AutoFusionPro.Domain.Interfaces.Repository;
using AutoFusionPro.Domain.Models;
using AutoFusionPro.Infrastructure.Data.Context;
using AutoFusionPro.Infrastructure.Data.Repositories.Base;
using Microsoft.Extensions.Logging;

namespace AutoFusionPro.Infrastructure.Data.Repositories
{
    public class CategoryRepository : Repository<Category, CategoryRepository>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;


        public CategoryRepository(ApplicationDbContext context, ILogger<CategoryRepository> logger) : base(context, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }
    }
}

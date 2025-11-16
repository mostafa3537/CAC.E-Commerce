using Microsoft.EntityFrameworkCore;
using CAC.Domain.Entities;
using CAC.Domain.Common;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace CAC.Infrastrucure;

public class ApplicationDbContext : DbContext
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
		_httpContextAccessor = httpContextAccessor;
	}

	public DbSet<User> Users { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

		ApplySoftDeleteFilters(modelBuilder);

		ConfigureAuditedEntities(modelBuilder);
	}

	private void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
	{
		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		{
			if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
			{
				var softDeleteMethod = typeof(ApplicationDbContext)?
					.GetMethod(nameof(ApplySoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)?
					.MakeGenericMethod(entityType.ClrType);

				softDeleteMethod?.Invoke(null, [modelBuilder]);
			}
		}
	}

	private static void ConfigureAuditedEntities(ModelBuilder modelBuilder)
	{
		var auditedEntities = modelBuilder.Model.GetEntityTypes()
				   .Where(e => typeof(IAuditedEntity).IsAssignableFrom(e.ClrType));

		foreach (var entity in auditedEntities)
		{
			modelBuilder.Entity(entity.ClrType)
				.Property(nameof(IAuditedEntity.CreatedBy))
				.HasMaxLength(250);

			modelBuilder.Entity(entity.ClrType)
				.Property(nameof(IAuditedEntity.UpdatedBy))
				.HasMaxLength(250);
		}
	}

	private static void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
		where TEntity : class, ISoftDelete
	{
		modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
	}

	public override int SaveChanges()
	{
		throw new NotImplementedException("please use SaveChangesAsync.");
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		SetAuditedEntityProperties();
		return await base.SaveChangesAsync(cancellationToken);
	}

	private void SetAuditedEntityProperties()
	{
		var UserId = _httpContextAccessor.HttpContext?.Items["UserId"] as string;

		var now = DateTime.UtcNow;

		foreach (var entry in ChangeTracker.Entries<IAuditedEntity>())
		{
			var entity = entry.Entity;

			if (entry.State == EntityState.Added)
			{
				entity.CreationDate = now;
				entity.CreatedBy = UserId;
				entity.UpdatedBy = UserId;
			}
			else if (entry.State == EntityState.Modified)
			{
				entity.UpdationDate = now;
				entity.UpdatedBy = UserId;
			}
		}
	}

}


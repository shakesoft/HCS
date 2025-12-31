using HC.WorkflowTemplates;
using HC.Workflows;
using HC.WorkflowDefinitions;
using HC.MasterDatas;
using HC.Positions;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace HC.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class HCTenantDbContext : HCDbContextBase<HCTenantDbContext>
{
    public DbSet<WorkflowTemplate> WorkflowTemplates { get; set; } = null!;
    public DbSet<Workflow> Workflows { get; set; } = null!;
    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; } = null!;
    public DbSet<MasterData> MasterDatas { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;

    public HCTenantDbContext(DbContextOptions<HCTenantDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.SetMultiTenancySide(MultiTenancySides.Tenant);
        base.OnModelCreating(builder);
        builder.Entity<Position>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "Positions", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(Position.TenantId));
            b.Property(x => x.Code).HasColumnName(nameof(Position.Code)).IsRequired().HasMaxLength(PositionConsts.CodeMaxLength);
            b.Property(x => x.Name).HasColumnName(nameof(Position.Name)).IsRequired();
            b.Property(x => x.SignOrder).HasColumnName(nameof(Position.SignOrder)).HasMaxLength(PositionConsts.SignOrderMaxLength);
            b.Property(x => x.IsActive).HasColumnName(nameof(Position.IsActive));
        });
        builder.Entity<MasterData>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "MasterDatas", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(MasterData.TenantId));
            b.Property(x => x.Type).HasColumnName(nameof(MasterData.Type)).IsRequired().HasMaxLength(MasterDataConsts.TypeMaxLength);
            b.Property(x => x.Code).HasColumnName(nameof(MasterData.Code)).IsRequired().HasMaxLength(MasterDataConsts.CodeMaxLength);
            b.Property(x => x.Name).HasColumnName(nameof(MasterData.Name)).IsRequired();
            b.Property(x => x.SortOrder).HasColumnName(nameof(MasterData.SortOrder)).HasMaxLength(MasterDataConsts.SortOrderMaxLength);
            b.Property(x => x.IsActive).HasColumnName(nameof(MasterData.IsActive));
        });
        builder.Entity<WorkflowDefinition>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "WorkflowDefinitions", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(WorkflowDefinition.TenantId));
            b.Property(x => x.Code).HasColumnName(nameof(WorkflowDefinition.Code)).IsRequired().HasMaxLength(WorkflowDefinitionConsts.CodeMaxLength);
            b.Property(x => x.Name).HasColumnName(nameof(WorkflowDefinition.Name)).IsRequired();
            b.Property(x => x.Description).HasColumnName(nameof(WorkflowDefinition.Description));
            b.Property(x => x.IsActive).HasColumnName(nameof(WorkflowDefinition.IsActive));
        });
        builder.Entity<Workflow>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "Workflows", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(Workflow.TenantId));
            b.Property(x => x.Code).HasColumnName(nameof(Workflow.Code)).IsRequired().HasMaxLength(WorkflowConsts.CodeMaxLength);
            b.Property(x => x.Name).HasColumnName(nameof(Workflow.Name)).IsRequired();
            b.Property(x => x.Description).HasColumnName(nameof(Workflow.Description));
            b.Property(x => x.IsActive).HasColumnName(nameof(Workflow.IsActive));
        });
        builder.Entity<WorkflowTemplate>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "WorkflowTemplates", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(WorkflowTemplate.TenantId));
            b.Property(x => x.Code).HasColumnName(nameof(WorkflowTemplate.Code)).IsRequired().HasMaxLength(WorkflowTemplateConsts.CodeMaxLength);
            b.Property(x => x.Name).HasColumnName(nameof(WorkflowTemplate.Name)).IsRequired();
            b.Property(x => x.WordTemplatePath).HasColumnName(nameof(WorkflowTemplate.WordTemplatePath));
            b.Property(x => x.ContentSchema).HasColumnName(nameof(WorkflowTemplate.ContentSchema));
            b.Property(x => x.OutputFormat).HasColumnName(nameof(WorkflowTemplate.OutputFormat)).HasMaxLength(WorkflowTemplateConsts.OutputFormatMaxLength);
            b.Property(x => x.SignMode).HasColumnName(nameof(WorkflowTemplate.SignMode)).HasMaxLength(WorkflowTemplateConsts.SignModeMaxLength);
            b.HasOne<Workflow>().WithMany().IsRequired().HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.NoAction);
        });
    }
}
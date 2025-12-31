using HC.ProjectTaskAssignments;
using HC.ProjectTasks;
using HC.ProjectMembers;
using HC.Projects;
using HC.DocumentHistories;
using HC.DocumentAssignments;
using HC.DocumentWorkflowInstances;
using HC.DocumentFiles;
using HC.Documents;
using HC.WorkflowStepAssignments;
using HC.Units;
using HC.Departments;
using Volo.Abp.Identity;
using HC.WorkflowStepTemplates;
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
public class HCDbContext : HCDbContextBase<HCDbContext>
{
    public DbSet<ProjectTaskAssignment> ProjectTaskAssignments { get; set; } = null!;
    public DbSet<ProjectTask> ProjectTasks { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<DocumentHistory> DocumentHistories { get; set; } = null!;
    public DbSet<DocumentAssignment> DocumentAssignments { get; set; } = null!;
    public DbSet<DocumentWorkflowInstance> DocumentWorkflowInstances { get; set; } = null!;
    public DbSet<DocumentFile> DocumentFiles { get; set; } = null!;
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<WorkflowStepAssignment> WorkflowStepAssignments { get; set; } = null!;
    public DbSet<Unit> Units { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<WorkflowStepTemplate> WorkflowStepTemplates { get; set; } = null!;
    public DbSet<WorkflowTemplate> WorkflowTemplates { get; set; } = null!;
    public DbSet<Workflow> Workflows { get; set; } = null!;
    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; } = null!;
    public DbSet<MasterData> MasterDatas { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;

    public HCDbContext(DbContextOptions<HCDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.SetMultiTenancySide(MultiTenancySides.Both);
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
        builder.Entity<WorkflowStepTemplate>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "WorkflowStepTemplates", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(WorkflowStepTemplate.TenantId));
            b.Property(x => x.Order).HasColumnName(nameof(WorkflowStepTemplate.Order)).IsRequired().HasMaxLength(WorkflowStepTemplateConsts.OrderMaxLength);
            b.Property(x => x.Name).HasColumnName(nameof(WorkflowStepTemplate.Name)).IsRequired();
            b.Property(x => x.Type).HasColumnName(nameof(WorkflowStepTemplate.Type)).IsRequired().HasMaxLength(WorkflowStepTemplateConsts.TypeMaxLength);
            b.Property(x => x.SLADays).HasColumnName(nameof(WorkflowStepTemplate.SLADays));
            b.Property(x => x.AllowReturn).HasColumnName(nameof(WorkflowStepTemplate.AllowReturn));
            b.Property(x => x.IsActive).HasColumnName(nameof(WorkflowStepTemplate.IsActive));
            b.HasOne<Workflow>().WithMany().IsRequired().HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<Department>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "Departments", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(Department.TenantId));
            b.Property(x => x.Code).HasColumnName(nameof(Department.Code)).IsRequired().HasMaxLength(DepartmentConsts.CodeMaxLength);
            b.Property(x => x.Name).HasColumnName(nameof(Department.Name)).IsRequired();
            b.Property(x => x.ParentId).HasColumnName(nameof(Department.ParentId));
            b.Property(x => x.Level).HasColumnName(nameof(Department.Level));
            b.Property(x => x.SortOrder).HasColumnName(nameof(Department.SortOrder));
            b.Property(x => x.IsActive).HasColumnName(nameof(Department.IsActive));
            b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.LeaderUserId).OnDelete(DeleteBehavior.SetNull);
        });
        builder.Entity<Unit>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "Units", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(Unit.TenantId));
            b.Property(x => x.Code).HasColumnName(nameof(Unit.Code)).IsRequired().HasMaxLength(UnitConsts.CodeMaxLength);
            b.Property(x => x.Name).HasColumnName(nameof(Unit.Name)).IsRequired();
            b.Property(x => x.SortOrder).HasColumnName(nameof(Unit.SortOrder));
            b.Property(x => x.IsActive).HasColumnName(nameof(Unit.IsActive));
        });
        builder.Entity<WorkflowStepAssignment>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "WorkflowStepAssignments", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(WorkflowStepAssignment.TenantId));
            b.Property(x => x.IsPrimary).HasColumnName(nameof(WorkflowStepAssignment.IsPrimary));
            b.Property(x => x.IsActive).HasColumnName(nameof(WorkflowStepAssignment.IsActive));
            b.HasOne<Workflow>().WithMany().HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne<WorkflowStepTemplate>().WithMany().HasForeignKey(x => x.StepId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne<WorkflowTemplate>().WithMany().HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.DefaultUserId).OnDelete(DeleteBehavior.SetNull);
        });
        builder.Entity<Document>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "Documents", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(Document.TenantId));
            b.Property(x => x.No).HasColumnName(nameof(Document.No)).HasMaxLength(DocumentConsts.NoMaxLength);
            b.Property(x => x.Title).HasColumnName(nameof(Document.Title)).IsRequired();
            b.Property(x => x.Type).HasColumnName(nameof(Document.Type)).HasMaxLength(DocumentConsts.TypeMaxLength);
            b.Property(x => x.UrgencyLevel).HasColumnName(nameof(Document.UrgencyLevel)).HasMaxLength(DocumentConsts.UrgencyLevelMaxLength);
            b.Property(x => x.SecrecyLevel).HasColumnName(nameof(Document.SecrecyLevel)).HasMaxLength(DocumentConsts.SecrecyLevelMaxLength);
            b.Property(x => x.CurrentStatus).HasColumnName(nameof(Document.CurrentStatus)).HasMaxLength(DocumentConsts.CurrentStatusMaxLength);
            b.Property(x => x.CompletedTime).HasColumnName(nameof(Document.CompletedTime));
            b.HasOne<MasterData>().WithMany().HasForeignKey(x => x.FieldId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne<Unit>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne<Workflow>().WithMany().HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne<MasterData>().WithMany().HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.SetNull);
        });
        builder.Entity<DocumentFile>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "DocumentFiles", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(DocumentFile.TenantId));
            b.Property(x => x.Name).HasColumnName(nameof(DocumentFile.Name)).IsRequired();
            b.Property(x => x.Path).HasColumnName(nameof(DocumentFile.Path));
            b.Property(x => x.Hash).HasColumnName(nameof(DocumentFile.Hash));
            b.Property(x => x.IsSigned).HasColumnName(nameof(DocumentFile.IsSigned));
            b.Property(x => x.UploadedAt).HasColumnName(nameof(DocumentFile.UploadedAt));
            b.HasOne<Document>().WithMany().IsRequired().HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<DocumentWorkflowInstance>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "DocumentWorkflowInstances", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(DocumentWorkflowInstance.TenantId));
            b.Property(x => x.Status).HasColumnName(nameof(DocumentWorkflowInstance.Status)).IsRequired().HasMaxLength(DocumentWorkflowInstanceConsts.StatusMaxLength);
            b.Property(x => x.StartedAt).HasColumnName(nameof(DocumentWorkflowInstance.StartedAt));
            b.Property(x => x.FinishedAt).HasColumnName(nameof(DocumentWorkflowInstance.FinishedAt));
            b.HasOne<Document>().WithMany().IsRequired().HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<Workflow>().WithMany().IsRequired().HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<WorkflowTemplate>().WithMany().IsRequired().HasForeignKey(x => x.WorkflowTemplateId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<WorkflowStepTemplate>().WithMany().IsRequired().HasForeignKey(x => x.CurrentStepId).OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<DocumentAssignment>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "DocumentAssignments", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(DocumentAssignment.TenantId));
            b.Property(x => x.StepOrder).HasColumnName(nameof(DocumentAssignment.StepOrder)).HasMaxLength(DocumentAssignmentConsts.StepOrderMaxLength);
            b.Property(x => x.ActionType).HasColumnName(nameof(DocumentAssignment.ActionType)).IsRequired().HasMaxLength(DocumentAssignmentConsts.ActionTypeMaxLength);
            b.Property(x => x.Status).HasColumnName(nameof(DocumentAssignment.Status)).IsRequired().HasMaxLength(DocumentAssignmentConsts.StatusMaxLength);
            b.Property(x => x.AssignedAt).HasColumnName(nameof(DocumentAssignment.AssignedAt));
            b.Property(x => x.ProcessedAt).HasColumnName(nameof(DocumentAssignment.ProcessedAt));
            b.Property(x => x.IsCurrent).HasColumnName(nameof(DocumentAssignment.IsCurrent));
            b.HasOne<Document>().WithMany().IsRequired().HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<WorkflowStepTemplate>().WithMany().IsRequired().HasForeignKey(x => x.StepId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<IdentityUser>().WithMany().IsRequired().HasForeignKey(x => x.ReceiverUserId).OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<DocumentHistory>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "DocumentHistories", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(DocumentHistory.TenantId));
            b.Property(x => x.Comment).HasColumnName(nameof(DocumentHistory.Comment));
            b.Property(x => x.Action).HasColumnName(nameof(DocumentHistory.Action)).IsRequired().HasMaxLength(DocumentHistoryConsts.ActionMaxLength);
            b.HasOne<Document>().WithMany().IsRequired().HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.FromUser).OnDelete(DeleteBehavior.SetNull);
            b.HasOne<IdentityUser>().WithMany().IsRequired().HasForeignKey(x => x.ToUser).OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<Project>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "Projects", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(Project.TenantId));
            b.Property(x => x.Code).HasColumnName(nameof(Project.Code)).IsRequired().HasMaxLength(ProjectConsts.CodeMaxLength);
            b.Property(x => x.Name).HasColumnName(nameof(Project.Name)).IsRequired().HasMaxLength(ProjectConsts.NameMaxLength);
            b.Property(x => x.Description).HasColumnName(nameof(Project.Description));
            b.Property(x => x.StartDate).HasColumnName(nameof(Project.StartDate));
            b.Property(x => x.EndDate).HasColumnName(nameof(Project.EndDate));
            b.Property(x => x.Status).HasColumnName(nameof(Project.Status)).IsRequired().HasMaxLength(ProjectConsts.StatusMaxLength);
            b.HasOne<Department>().WithMany().HasForeignKey(x => x.OwnerDepartmentId).OnDelete(DeleteBehavior.SetNull);
        });
        builder.Entity<ProjectMember>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "ProjectMembers", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(ProjectMember.TenantId));
            b.Property(x => x.MemberRole).HasColumnName(nameof(ProjectMember.MemberRole)).IsRequired().HasMaxLength(ProjectMemberConsts.MemberRoleMaxLength);
            b.Property(x => x.JoinedAt).HasColumnName(nameof(ProjectMember.JoinedAt));
            b.HasOne<Project>().WithMany().IsRequired().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<IdentityUser>().WithMany().IsRequired().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<ProjectTask>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "ProjectTasks", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(ProjectTask.TenantId));
            b.Property(x => x.ParentTaskId).HasColumnName(nameof(ProjectTask.ParentTaskId));
            b.Property(x => x.Code).HasColumnName(nameof(ProjectTask.Code)).IsRequired().HasMaxLength(ProjectTaskConsts.CodeMaxLength);
            b.Property(x => x.Title).HasColumnName(nameof(ProjectTask.Title)).IsRequired().HasMaxLength(ProjectTaskConsts.TitleMaxLength);
            b.Property(x => x.Description).HasColumnName(nameof(ProjectTask.Description));
            b.Property(x => x.StartDate).HasColumnName(nameof(ProjectTask.StartDate));
            b.Property(x => x.DueDate).HasColumnName(nameof(ProjectTask.DueDate));
            b.Property(x => x.Priority).HasColumnName(nameof(ProjectTask.Priority)).IsRequired().HasMaxLength(ProjectTaskConsts.PriorityMaxLength);
            b.Property(x => x.Status).HasColumnName(nameof(ProjectTask.Status)).IsRequired().HasMaxLength(ProjectTaskConsts.StatusMaxLength);
            b.Property(x => x.ProgressPercent).HasColumnName(nameof(ProjectTask.ProgressPercent)).HasMaxLength(ProjectTaskConsts.ProgressPercentMaxLength);
            b.HasOne<Project>().WithMany().IsRequired().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.NoAction);
        });
        builder.Entity<ProjectTaskAssignment>(b => {
            b.ToTable(HCConsts.DbTablePrefix + "ProjectTaskAssignments", HCConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).HasColumnName(nameof(ProjectTaskAssignment.TenantId));
            b.Property(x => x.AssignmentRole).HasColumnName(nameof(ProjectTaskAssignment.AssignmentRole)).IsRequired().HasMaxLength(ProjectTaskAssignmentConsts.AssignmentRoleMaxLength);
            b.Property(x => x.AssignedAt).HasColumnName(nameof(ProjectTaskAssignment.AssignedAt));
            b.Property(x => x.Note).HasColumnName(nameof(ProjectTaskAssignment.Note));
            b.HasOne<ProjectTask>().WithMany().IsRequired().HasForeignKey(x => x.ProjectTaskId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne<IdentityUser>().WithMany().IsRequired().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        });
    }
}
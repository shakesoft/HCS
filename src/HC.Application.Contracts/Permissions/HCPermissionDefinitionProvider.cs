using HC.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace HC.Permissions;

public class HCPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(HCPermissions.GroupName);
        myGroup.AddPermission(HCPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(HCPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);
        var booksPermission = myGroup.AddPermission(HCPermissions.Books.Default, L("Permission:Books"));
        booksPermission.AddChild(HCPermissions.Books.Create, L("Permission:Books.Create"));
        booksPermission.AddChild(HCPermissions.Books.Edit, L("Permission:Books.Edit"));
        booksPermission.AddChild(HCPermissions.Books.Delete, L("Permission:Books.Delete"));
        //Define your own permissions here. Example:
        //myGroup.AddPermission(HCPermissions.MyPermission1, L("Permission:MyPermission1"));
        var positionPermission = myGroup.AddPermission(HCPermissions.Positions.Default, L("Permission:Positions"));
        positionPermission.AddChild(HCPermissions.Positions.Create, L("Permission:Create"));
        positionPermission.AddChild(HCPermissions.Positions.Edit, L("Permission:Edit"));
        positionPermission.AddChild(HCPermissions.Positions.Delete, L("Permission:Delete"));
        var masterDataPermission = myGroup.AddPermission(HCPermissions.MasterDatas.Default, L("Permission:MasterDatas"));
        masterDataPermission.AddChild(HCPermissions.MasterDatas.Create, L("Permission:Create"));
        masterDataPermission.AddChild(HCPermissions.MasterDatas.Edit, L("Permission:Edit"));
        masterDataPermission.AddChild(HCPermissions.MasterDatas.Delete, L("Permission:Delete"));
        var workflowDefinitionPermission = myGroup.AddPermission(HCPermissions.WorkflowDefinitions.Default, L("Permission:WorkflowDefinitions"));
        workflowDefinitionPermission.AddChild(HCPermissions.WorkflowDefinitions.Create, L("Permission:Create"));
        workflowDefinitionPermission.AddChild(HCPermissions.WorkflowDefinitions.Edit, L("Permission:Edit"));
        workflowDefinitionPermission.AddChild(HCPermissions.WorkflowDefinitions.Delete, L("Permission:Delete"));
        var workflowPermission = myGroup.AddPermission(HCPermissions.Workflows.Default, L("Permission:Workflows"));
        workflowPermission.AddChild(HCPermissions.Workflows.Create, L("Permission:Create"));
        workflowPermission.AddChild(HCPermissions.Workflows.Edit, L("Permission:Edit"));
        workflowPermission.AddChild(HCPermissions.Workflows.Delete, L("Permission:Delete"));
        var workflowTemplatePermission = myGroup.AddPermission(HCPermissions.WorkflowTemplates.Default, L("Permission:WorkflowTemplates"));
        workflowTemplatePermission.AddChild(HCPermissions.WorkflowTemplates.Create, L("Permission:Create"));
        workflowTemplatePermission.AddChild(HCPermissions.WorkflowTemplates.Edit, L("Permission:Edit"));
        workflowTemplatePermission.AddChild(HCPermissions.WorkflowTemplates.Delete, L("Permission:Delete"));
        var workflowStepTemplatePermission = myGroup.AddPermission(HCPermissions.WorkflowStepTemplates.Default, L("Permission:WorkflowStepTemplates"));
        workflowStepTemplatePermission.AddChild(HCPermissions.WorkflowStepTemplates.Create, L("Permission:Create"));
        workflowStepTemplatePermission.AddChild(HCPermissions.WorkflowStepTemplates.Edit, L("Permission:Edit"));
        workflowStepTemplatePermission.AddChild(HCPermissions.WorkflowStepTemplates.Delete, L("Permission:Delete"));
        var departmentPermission = myGroup.AddPermission(HCPermissions.Departments.Default, L("Permission:Departments"));
        departmentPermission.AddChild(HCPermissions.Departments.Create, L("Permission:Create"));
        departmentPermission.AddChild(HCPermissions.Departments.Edit, L("Permission:Edit"));
        departmentPermission.AddChild(HCPermissions.Departments.Delete, L("Permission:Delete"));
        var unitPermission = myGroup.AddPermission(HCPermissions.Units.Default, L("Permission:Units"));
        unitPermission.AddChild(HCPermissions.Units.Create, L("Permission:Create"));
        unitPermission.AddChild(HCPermissions.Units.Edit, L("Permission:Edit"));
        unitPermission.AddChild(HCPermissions.Units.Delete, L("Permission:Delete"));
        var workflowStepAssignmentPermission = myGroup.AddPermission(HCPermissions.WorkflowStepAssignments.Default, L("Permission:WorkflowStepAssignments"));
        workflowStepAssignmentPermission.AddChild(HCPermissions.WorkflowStepAssignments.Create, L("Permission:Create"));
        workflowStepAssignmentPermission.AddChild(HCPermissions.WorkflowStepAssignments.Edit, L("Permission:Edit"));
        workflowStepAssignmentPermission.AddChild(HCPermissions.WorkflowStepAssignments.Delete, L("Permission:Delete"));
        var documentPermission = myGroup.AddPermission(HCPermissions.Documents.Default, L("Permission:Documents"));
        documentPermission.AddChild(HCPermissions.Documents.Create, L("Permission:Create"));
        documentPermission.AddChild(HCPermissions.Documents.Edit, L("Permission:Edit"));
        documentPermission.AddChild(HCPermissions.Documents.Delete, L("Permission:Delete"));
        var documentFilePermission = myGroup.AddPermission(HCPermissions.DocumentFiles.Default, L("Permission:DocumentFiles"));
        documentFilePermission.AddChild(HCPermissions.DocumentFiles.Create, L("Permission:Create"));
        documentFilePermission.AddChild(HCPermissions.DocumentFiles.Edit, L("Permission:Edit"));
        documentFilePermission.AddChild(HCPermissions.DocumentFiles.Delete, L("Permission:Delete"));
        var documentWorkflowInstancePermission = myGroup.AddPermission(HCPermissions.DocumentWorkflowInstances.Default, L("Permission:DocumentWorkflowInstances"));
        documentWorkflowInstancePermission.AddChild(HCPermissions.DocumentWorkflowInstances.Create, L("Permission:Create"));
        documentWorkflowInstancePermission.AddChild(HCPermissions.DocumentWorkflowInstances.Edit, L("Permission:Edit"));
        documentWorkflowInstancePermission.AddChild(HCPermissions.DocumentWorkflowInstances.Delete, L("Permission:Delete"));
        var documentAssignmentPermission = myGroup.AddPermission(HCPermissions.DocumentAssignments.Default, L("Permission:DocumentAssignments"));
        documentAssignmentPermission.AddChild(HCPermissions.DocumentAssignments.Create, L("Permission:Create"));
        documentAssignmentPermission.AddChild(HCPermissions.DocumentAssignments.Edit, L("Permission:Edit"));
        documentAssignmentPermission.AddChild(HCPermissions.DocumentAssignments.Delete, L("Permission:Delete"));
        var documentHistoryPermission = myGroup.AddPermission(HCPermissions.DocumentHistories.Default, L("Permission:DocumentHistories"));
        documentHistoryPermission.AddChild(HCPermissions.DocumentHistories.Create, L("Permission:Create"));
        documentHistoryPermission.AddChild(HCPermissions.DocumentHistories.Edit, L("Permission:Edit"));
        documentHistoryPermission.AddChild(HCPermissions.DocumentHistories.Delete, L("Permission:Delete"));
        var projectPermission = myGroup.AddPermission(HCPermissions.Projects.Default, L("Permission:Projects"));
        projectPermission.AddChild(HCPermissions.Projects.Create, L("Permission:Create"));
        projectPermission.AddChild(HCPermissions.Projects.Edit, L("Permission:Edit"));
        projectPermission.AddChild(HCPermissions.Projects.Delete, L("Permission:Delete"));
        var projectMemberPermission = myGroup.AddPermission(HCPermissions.ProjectMembers.Default, L("Permission:ProjectMembers"));
        projectMemberPermission.AddChild(HCPermissions.ProjectMembers.Create, L("Permission:Create"));
        projectMemberPermission.AddChild(HCPermissions.ProjectMembers.Edit, L("Permission:Edit"));
        projectMemberPermission.AddChild(HCPermissions.ProjectMembers.Delete, L("Permission:Delete"));
        var taskPermission = myGroup.AddPermission(HCPermissions.Tasks.Default, L("Permission:Tasks"));
        taskPermission.AddChild(HCPermissions.Tasks.Create, L("Permission:Create"));
        taskPermission.AddChild(HCPermissions.Tasks.Edit, L("Permission:Edit"));
        taskPermission.AddChild(HCPermissions.Tasks.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HCResource>(name);
    }
}
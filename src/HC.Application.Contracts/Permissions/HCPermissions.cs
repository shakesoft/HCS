namespace HC.Permissions;

public static class HCPermissions
{
    public const string GroupName = "HC";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ".Dashboard";
        public const string Host = DashboardGroup + ".Host";
        public const string Tenant = DashboardGroup + ".Tenant";
    }

    public static class Books
    {
        public const string Default = GroupName + ".Books";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
    public static class Positions
    {
        public const string Default = GroupName + ".Positions";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class MasterDatas
    {
        public const string Default = GroupName + ".MasterDatas";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class WorkflowDefinitions
    {
        public const string Default = GroupName + ".WorkflowDefinitions";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Workflows
    {
        public const string Default = GroupName + ".Workflows";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class WorkflowTemplates
    {
        public const string Default = GroupName + ".WorkflowTemplates";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class WorkflowStepTemplates
    {
        public const string Default = GroupName + ".WorkflowStepTemplates";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Departments
    {
        public const string Default = GroupName + ".Departments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Units
    {
        public const string Default = GroupName + ".Units";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class WorkflowStepAssignments
    {
        public const string Default = GroupName + ".WorkflowStepAssignments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Documents
    {
        public const string Default = GroupName + ".Documents";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
        public const string SubmitForSigning = Default + ".SubmitForSigning";
    }

    public static class DocumentFiles
    {
        public const string Default = GroupName + ".DocumentFiles";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class DocumentWorkflowInstances
    {
        public const string Default = GroupName + ".DocumentWorkflowInstances";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class DocumentAssignments
    {
        public const string Default = GroupName + ".DocumentAssignments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class DocumentHistories
    {
        public const string Default = GroupName + ".DocumentHistories";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Projects
    {
        public const string Default = GroupName + ".Projects";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class ProjectMembers
    {
        public const string Default = GroupName + ".ProjectMembers";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Tasks
    {
        public const string Default = GroupName + ".Tasks";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class ProjectTasks
    {
        public const string Default = GroupName + ".ProjectTasks";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class ProjectTaskAssignments
    {
        public const string Default = GroupName + ".ProjectTaskAssignments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class ProjectTaskDocuments
    {
        public const string Default = GroupName + ".ProjectTaskDocuments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}
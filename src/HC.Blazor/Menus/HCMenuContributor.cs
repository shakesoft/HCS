using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using HC.Localization;
using HC.Permissions;
using HC.MultiTenancy;
using Volo.Abp.Users;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.FileManagement.Blazor.Navigation;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Saas.Host.Blazor.Navigation;

namespace HC.Blazor.Menus;

public class HCMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public HCMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<HCResource>();
        context.Menu.Items.Insert(0, new ApplicationMenuItem(HCMenus.Home, l["Menu:Home"], "/", icon: "fas fa-home", order: 1));
        context.Menu.AddItem(new ApplicationMenuItem("Documents", l["Menu:Documents"], icon: "fa fa-book", order: 2).AddItem(new ApplicationMenuItem("Documents.List", l["Menu:DocumentList"], url: "/documents").RequirePermissions(HCPermissions.Documents.Default)).AddItem(new ApplicationMenuItem("Documents.Create", l["Action.Create"], url: "/documents").RequirePermissions(HCPermissions.Documents.Create)).AddItem(new ApplicationMenuItem("Documents.Create", l["Action.SubmitForSigning"], url: "/documents").RequirePermissions(HCPermissions.Documents.SubmitForSigning)).AddItem(new ApplicationMenuItem("Documents.FileManagement", l["FileManagement"], url: "/documents").RequirePermissions(HCPermissions.DocumentFiles.Default)));
        context.Menu.AddItem(new ApplicationMenuItem("Workflows", l["Menu:Workflows"], icon: "fa fa-arrow-trend-up", order: 3).AddItem(new ApplicationMenuItem("Workflows.List", l["Menu:WorkflowList"], url: "/workflows").RequirePermissions(HCPermissions.Workflows.Default)).AddItem(new ApplicationMenuItem("Workflows.WorkflowTemplates", l["Menu:WorkflowTemplates"], url: "/workflow-templates").RequirePermissions(HCPermissions.WorkflowTemplates.Default)).AddItem(new ApplicationMenuItem("Workflows.WorkflowStepTemplates", l["Menu:WorkflowStepTemplates"], url: "/workflow-step-templates").RequirePermissions(HCPermissions.WorkflowStepTemplates.Default)).AddItem(new ApplicationMenuItem("Workflows.WorkflowStepAssignments", l["Menu:WorkflowStepAssignments"], url: "/workflow-step-assignments").RequirePermissions(HCPermissions.WorkflowStepAssignments.Default)).AddItem(new ApplicationMenuItem("Workflows.Follow", l["Workflows.Follow"], url: "/workflows").RequirePermissions(HCPermissions.Workflows.Default)));
        context.Menu.AddItem(new ApplicationMenuItem("Projects", l["Menu:Projects"], icon: "fa fa-diagram-project", order: 4).AddItem(new ApplicationMenuItem("Projects.List", l["Menu:ProjectList"], url: "/projects").RequirePermissions(HCPermissions.Projects.Default)).AddItem(new ApplicationMenuItem("Projects.ProjectDetail", l["Menu:ProjectDetail"], url: "/project-details").RequirePermissions(HCPermissions.Projects.Default)).AddItem(new ApplicationMenuItem("ProjectTasks.List", l["Menu:ProjectTaskList"], url: "/project-tasks").RequirePermissions(HCPermissions.Projects.Default)).AddItem(new ApplicationMenuItem("ProjectTasks.ProjectTaskByProjects", l["Menu:ProjectTaskByProjects"], url: "/project-tasks-by-project").RequirePermissions(HCPermissions.Projects.Default)).AddItem(new ApplicationMenuItem("ProjectTasks.ProjectTaskDetail", l["Menu:ProjectTaskDetail"], url: "/project-tasks-details").RequirePermissions(HCPermissions.Projects.Default)));
        // context.Menu.AddItem(new ApplicationMenuItem("ProjectTasks",
        //     l["Menu:ProjectTasks"], icon: "fa fa-list-check", order: 5)
        //         .AddItem(
        //             new ApplicationMenuItem("ProjectTasks.List", l["Menu:ProjectTaskList"], url: "/project-tasks")
        //             .RequirePermissions(HCPermissions.Projects.Default))
        //         .AddItem(
        //             new ApplicationMenuItem("ProjectTasks.ProjectTaskByProjects", l["Menu:ProjectTaskByProjects"], url: "/project-tasks-by-project")
        //             .RequirePermissions(HCPermissions.Projects.Default))
        //         .AddItem(
        //             new ApplicationMenuItem("ProjectTasks.ProjectTaskDetail", l["Menu:ProjectTaskDetail"], url: "/project-tasks-details")
        //             .RequirePermissions(HCPermissions.Projects.Default))
        //         );
        context.Menu.AddItem(new ApplicationMenuItem("CalendarAndEvents", l["Menu:CalendarAndEvents"], icon: "fa fa-calendar-days", order: 6).AddItem(new ApplicationMenuItem("CalendarAndEvents.PersonalCalendars", l["Menu:PersonalCalendars"], url: "/project-tasks").RequirePermissions(HCPermissions.Projects.Default)).AddItem(new ApplicationMenuItem("CalendarAndEvents.ProjectAndTaskCalendars", l["Menu:ProjectAndTaskCalendars"], url: "/project-tasks-by-project").RequirePermissions(HCPermissions.Projects.Default)).AddItem(new ApplicationMenuItem("CalendarAndEvents.EventManagements", l["Menu:EventManagements"], url: "/project-tasks-details").RequirePermissions(HCPermissions.Projects.Default)));
        // Temporarily disabled Chat feature
        // context.Menu.AddItem(new ApplicationMenuItem("Chats", l["Menu:Chats"], "/chat", icon: "fa fa-message", order: 7));
        context.Menu.AddItem(new ApplicationMenuItem("Notifications", l["Menu:Notifications"], icon: "fa fa-bell", order: 8).AddItem(new ApplicationMenuItem("Notifications.Read", l["Menu:NotificationsRead"], url: "/notifications-read").RequirePermissions(HCPermissions.Projects.Default)).AddItem(new ApplicationMenuItem("Notifications.UnRead", l["Menu:NotificationsUnRead"], url: "/notifications-unread").RequirePermissions(HCPermissions.Projects.Default)));
        context.Menu.AddItem(new ApplicationMenuItem("MasterDatas", l["Menu:Categories"], icon: "fa fa-layer-group", order: 9).AddItem(new ApplicationMenuItem("MasterDatas.DocumentTypes", l["DocumentTypes"], url: "/document-types").RequirePermissions(HCPermissions.MasterDatas.DocumentTypeDefault)).AddItem(new ApplicationMenuItem("MasterDatas.Sector", l["Sector"], url: "/sectors").RequirePermissions(HCPermissions.MasterDatas.SectorDefault))// .AddItem(
                                                                                                                                                                                                                                                                                                                                                                                                                                                           //     new ApplicationMenuItem("MasterDatas.Status", l["Status"], url: "/status")
                                                                                                                                                                                                                                                                                                                                                                                                                                                           //     .RequirePermissions(HCPermissions.MasterDatas.StatusDefault))
        .AddItem(new ApplicationMenuItem("MasterDatas.UrgencyLevel", l["UrgencyLevel"], url: "/urgency-levels").RequirePermissions(HCPermissions.MasterDatas.UrgencyLevelDefault)).AddItem(new ApplicationMenuItem("MasterDatas.ConfidentialityLevel", l["ConfidentialityLevel"], url: "/confidentiality-levels").RequirePermissions(HCPermissions.MasterDatas.ConfidentialityLevelDefault)).AddItem(new ApplicationMenuItem("MasterDatas.ProcessingMethod", l["ProcessingMethod"], url: "/processing-methods").RequirePermissions(HCPermissions.MasterDatas.ProcessingMethodDefault)).AddItem(new ApplicationMenuItem("MasterDatas.DocumentStatus", l["DocumentStatus"], url: "/document-status").RequirePermissions(HCPermissions.MasterDatas.DocumentStatusDefault)).AddItem(new ApplicationMenuItem("MasterDatas.SigningMethod", l["SigningMethod"], url: "/signing-methods").RequirePermissions(HCPermissions.MasterDatas.SigningMethodDefault)).AddItem(new ApplicationMenuItem("MasterDatas.EventType", l["EventType"], url: "/even-types").RequirePermissions(HCPermissions.MasterDatas.EventTypeDefault)).AddItem(new ApplicationMenuItem("MasterDatas.IssuingAuthority", l["IssuingAuthority"], url: "/units").RequirePermissions(HCPermissions.MasterDatas.UnitDefault)));
        context.Menu.AddItem(new ApplicationMenuItem("HRs", l["Menu:HRs"], icon: "fa fa-sitemap", order: 11)// .AddItem(
                                                                                                            //     new ApplicationMenuItem("HRs.Units", l["Unit"], url: "/units")
                                                                                                            //     .RequirePermissions(HCPermissions.Hrs.UnitDefault))
        .AddItem(new ApplicationMenuItem("HRs.Departments", l["Department"], url: "/departments").RequirePermissions(HCPermissions.Hrs.DepartmentDefault)));
        context.Menu.AddItem(new ApplicationMenuItem("Reports", l["Menu:Reports"], icon: "fa fa-chart-area", order: 12).AddItem(new ApplicationMenuItem("Reports.Documents", l["Documents"], url: "/reports-documents").RequirePermissions(HCPermissions.Reports.DocumentDefault)).AddItem(new ApplicationMenuItem("Reports.Projects", l["Projects"], url: "/reports-projects").RequirePermissions(HCPermissions.Reports.ProjectDefault)).AddItem(new ApplicationMenuItem("Reports.ProjectTasks", l["ProjectTasks"], url: "/reports-project-tasks").RequirePermissions(HCPermissions.Reports.ProjectTaskDefault)).AddItem(new ApplicationMenuItem("Reports.Workflows", l["Workflows"], url: "/reports-workflows").RequirePermissions(HCPermissions.Reports.WorkflowDefault)));
        //HostDashboard
        // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.HostDashboard, l["Menu:Dashboard"], "~/HostDashboard", icon: "fa fa-line-chart", order: 2).RequirePermissions(HCPermissions.Dashboard.Host));
        // //TenantDashboard
        // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.TenantDashboard, l["Menu:Dashboard"], "~/Dashboard", icon: "fa fa-line-chart", order: 2).RequirePermissions(HCPermissions.Dashboard.Tenant));
        // //Saas
        // context.Menu.SetSubItemOrder(SaasHostMenus.GroupName, 3);
        // //File management
        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 15;
        // context.Menu.SetSubItemOrder(FileManagementMenuNames.GroupName, 5);
        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 2);
        //Administration->OpenIddict
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 3);
        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 5);
        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 6);
        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 7);
        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 8);
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Positions, l["Menu:Positions"], url: "/positions", icon: "fa fa-code-branch", requiredPermissionName: HCPermissions.Positions.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.MasterDatas, l["Menu:MasterDatas"], url: "/master-datas", icon: "fa fa-table", requiredPermissionName: HCPermissions.MasterDatas.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.WorkflowDefinitions, l["Menu:WorkflowDefinitions"], url: "/workflow-definitions", icon: "fa fa-stream", requiredPermissionName: HCPermissions.WorkflowDefinitions.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Workflows, l["Menu:Workflows"], url: "/workflows", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Workflows.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.WorkflowTemplates, l["Menu:WorkflowTemplates"], url: "/workflow-templates", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.WorkflowTemplates.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.WorkflowStepTemplates, l["Menu:WorkflowStepTemplates"], url: "/workflow-step-templates", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.WorkflowStepTemplates.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Departments, l["Menu:Departments"], url: "/departments", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Departments.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Units, l["Menu:Units"], url: "/units", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Units.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.WorkflowStepAssignments, l["Menu:WorkflowStepAssignments"], url: "/workflow-step-assignments", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.WorkflowStepAssignments.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Documents, l["Menu:Documents"], url: "/documents", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Documents.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.DocumentFiles, l["Menu:DocumentFiles"], url: "/document-files", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.DocumentFiles.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.DocumentWorkflowInstances, l["Menu:DocumentWorkflowInstances"], url: "/document-workflow-instances", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.DocumentWorkflowInstances.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.DocumentAssignments, l["Menu:DocumentAssignments"], url: "/document-assignments", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.DocumentAssignments.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.DocumentHistories, l["Menu:DocumentHistories"], url: "/document-histories", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.DocumentHistories.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.ProjectMembers, l["Menu:ProjectMembers"], url: "/project-members", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.ProjectMembers.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Tasks, l["Menu:Tasks"], url: "/tasks", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Tasks.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.ProjectTasks, l["Menu:ProjectTasks"], url: "/project-tasks", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.ProjectTasks.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.ProjectTaskAssignments, l["Menu:ProjectTaskAssignments"], url: "/project-task-assignments", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.ProjectTaskAssignments.Default));
    // context.Menu.AddItem(new ApplicationMenuItem(HCMenus.ProjectTaskDocuments, l["Menu:ProjectTaskDocuments"], url: "/project-task-documents", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.ProjectTaskDocuments.Default));
        return Task.CompletedTask;
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var uiResource = context.GetLocalizer<AbpUiResource>();
        var accountResource = context.GetLocalizer<AccountResource>();
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "~";
        context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{authServerUrl.EnsureEndsWith('/')}Account/Manage", icon: "fa fa-cog", order: 1000, target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs", icon: "fa fa-user-shield", target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Sessions", accountResource["Sessions"], url: $"{authServerUrl.EnsureEndsWith('/')}Account/Sessions", icon: "fa fa-clock", target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Logout", uiResource["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000).RequireAuthenticated());
        return Task.CompletedTask;
    }
}
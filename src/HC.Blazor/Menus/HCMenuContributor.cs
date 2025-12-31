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
        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;
        //HostDashboard
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.HostDashboard, l["Menu:Dashboard"], "~/HostDashboard", icon: "fa fa-line-chart", order: 2).RequirePermissions(HCPermissions.Dashboard.Host));
        //TenantDashboard
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.TenantDashboard, l["Menu:Dashboard"], "~/Dashboard", icon: "fa fa-line-chart", order: 2).RequirePermissions(HCPermissions.Dashboard.Tenant));
        //Saas
        context.Menu.SetSubItemOrder(SaasHostMenus.GroupName, 3);
        //File management
        context.Menu.SetSubItemOrder(FileManagementMenuNames.GroupName, 5);
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
        context.Menu.AddItem(new ApplicationMenuItem("BooksStore", l["Menu:HC"], icon: "fa fa-book").AddItem(new ApplicationMenuItem("BooksStore.Books", l["Menu:Books"], url: "/books").RequirePermissions(HCPermissions.Books.Default)));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Positions, l["Menu:Positions"], url: "/positions", icon: "fa fa-code-branch", requiredPermissionName: HCPermissions.Positions.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.MasterDatas, l["Menu:MasterDatas"], url: "/master-datas", icon: "fa fa-table", requiredPermissionName: HCPermissions.MasterDatas.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.WorkflowDefinitions, l["Menu:WorkflowDefinitions"], url: "/workflow-definitions", icon: "fa fa-stream", requiredPermissionName: HCPermissions.WorkflowDefinitions.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Workflows, l["Menu:Workflows"], url: "/workflows", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Workflows.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.WorkflowTemplates, l["Menu:WorkflowTemplates"], url: "/workflow-templates", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.WorkflowTemplates.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.WorkflowStepTemplates, l["Menu:WorkflowStepTemplates"], url: "/workflow-step-templates", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.WorkflowStepTemplates.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Departments, l["Menu:Departments"], url: "/departments", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Departments.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Units, l["Menu:Units"], url: "/units", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Units.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.WorkflowStepAssignments, l["Menu:WorkflowStepAssignments"], url: "/workflow-step-assignments", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.WorkflowStepAssignments.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Documents, l["Menu:Documents"], url: "/documents", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Documents.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.DocumentFiles, l["Menu:DocumentFiles"], url: "/document-files", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.DocumentFiles.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.DocumentWorkflowInstances, l["Menu:DocumentWorkflowInstances"], url: "/document-workflow-instances", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.DocumentWorkflowInstances.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.DocumentAssignments, l["Menu:DocumentAssignments"], url: "/document-assignments", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.DocumentAssignments.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.DocumentHistories, l["Menu:DocumentHistories"], url: "/document-histories", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.DocumentHistories.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Projects, l["Menu:Projects"], url: "/projects", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Projects.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.ProjectMembers, l["Menu:ProjectMembers"], url: "/project-members", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.ProjectMembers.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.Tasks, l["Menu:Tasks"], url: "/tasks", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.Tasks.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.ProjectTasks, l["Menu:ProjectTasks"], url: "/project-tasks", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.ProjectTasks.Default));
        context.Menu.AddItem(new ApplicationMenuItem(HCMenus.ProjectTaskAssignments, l["Menu:ProjectTaskAssignments"], url: "/project-task-assignments", icon: "fa fa-file-alt", requiredPermissionName: HCPermissions.ProjectTaskAssignments.Default));
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
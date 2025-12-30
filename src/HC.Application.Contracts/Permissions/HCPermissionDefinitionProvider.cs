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
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HCResource>(name);
    }
}

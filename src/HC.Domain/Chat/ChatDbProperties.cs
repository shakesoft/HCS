using Volo.Abp.Data;

namespace HC.Chat;

public static class ChatDbProperties
{
    public static string DbTablePrefix { get; set; } = "Chat";

    public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

    public const string ConnectionStringName = "Default";
}

using Xunit;

namespace HC.EntityFrameworkCore;

[CollectionDefinition(HCTestConsts.CollectionDefinitionName)]
public class HCEntityFrameworkCoreCollection : ICollectionFixture<HCEntityFrameworkCoreFixture>
{

}

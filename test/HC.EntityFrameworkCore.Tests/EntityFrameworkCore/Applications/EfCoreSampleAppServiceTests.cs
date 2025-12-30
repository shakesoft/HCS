using HC.Samples;
using Xunit;

namespace HC.EntityFrameworkCore.Applications;

[Collection(HCTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<HCEntityFrameworkCoreTestModule>
{

}

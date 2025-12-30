using HC.Samples;
using Xunit;

namespace HC.EntityFrameworkCore.Domains;

[Collection(HCTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<HCEntityFrameworkCoreTestModule>
{

}

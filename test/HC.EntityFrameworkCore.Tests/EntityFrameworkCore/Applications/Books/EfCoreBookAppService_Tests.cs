using HC.Books;
using Xunit;

namespace HC.EntityFrameworkCore.Applications.Books;

[Collection(HCTestConsts.CollectionDefinitionName)]
public class EfCoreBookAppService_Tests : BookAppService_Tests<HCEntityFrameworkCoreTestModule>
{

}
using HC.WorkflowDefinitions;
using Xunit;
using HC.EntityFrameworkCore;

namespace HC.WorkflowDefinitions;

public class EfCoreWorkflowDefinitionsAppServiceTests : WorkflowDefinitionsAppServiceTests<HCEntityFrameworkCoreTestModule>
{
}
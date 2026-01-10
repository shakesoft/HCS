using HC.Departments;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.Projects;

namespace HC.Projects;

public class ProjectsDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly DepartmentsDataSeedContributor _departmentsDataSeedContributor;

    public ProjectsDataSeedContributor(IProjectRepository projectRepository, IUnitOfWorkManager unitOfWorkManager, DepartmentsDataSeedContributor departmentsDataSeedContributor)
    {
        _projectRepository = projectRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _departmentsDataSeedContributor = departmentsDataSeedContributor;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _departmentsDataSeedContributor.SeedAsync(context);
        await _projectRepository.InsertAsync(new Project(id: Guid.Parse("4b11cb6c-18c9-40b4-9baf-dacb948a5d88"), code: "9b8babb57d7c489696436b002151d9cf5ad26d70895b4e83a1", name: "bce877e523f14e8abcde45f67af5eef93376bee47f324ba49c62b3eed892be667883deea68c04942953546d661a773c688dc276883ff42c19cc57766b48f564643afa25136ec49d996385c30fcf4e29d0c24a97e1e124b9b881ac8299ccf5ad777712ea4f9f6456aba62433a8fc69e2f836c7aa287f74e91b1f872c9d18aa93", description: "a28117b024c44718a8bcea40c53c9b0aab9f4245e80941678c7a9f97607b993b", startDate: new DateTime(2001, 11, 22), endDate: new DateTime(2019, 11, 2), status: default, ownerDepartmentId: null));
        await _projectRepository.InsertAsync(new Project(id: Guid.Parse("da6f6522-d787-4df8-b903-44c0a5ab6c5b"), code: "980caee0d6b742699bd10f95bc70c2eb64e430eb1f7c405898", name: "72cbad25652c464e884dd119593b3f8e10931826fc304589b2b3d7a7bf4a5f084dffb5e7f85a4ea9b59336e235fdcade2b4d95b4b34e4cacaabd3976d768cb95fbd2c8cc4e7c47f4b1a71e6376b3bdb9ce5ee4548b924d0eaedba551928c518cee9d40ee8bcc4efcb6bf8037766eda3d93fae9e344034118a1d54197e85d298", description: "77efd4c5aa354af28a20eed97e36649fb4096fc3e2", startDate: new DateTime(2008, 9, 23), endDate: new DateTime(2009, 7, 19), status: default, ownerDepartmentId: null));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}
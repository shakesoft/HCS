using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.Departments;

namespace HC.Departments;

public class DepartmentsDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public DepartmentsDataSeedContributor(IDepartmentRepository departmentRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _departmentRepository = departmentRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _departmentRepository.InsertAsync(new Department(id: Guid.Parse("cef08c27-8872-40ce-ad09-bec261cd8502"), code: "36ba02f7df374080b70cda8a16d3bf324b98ee224bdb42cbaf", name: "faa5c9b038a94a42", parentId: "f3cfc4d937a94917b8143b15d73f59692cd3806a279041af884cfcf0cc1e95f778f5b26e6af", level: 156052469, sortOrder: 541749100, isActive: true, leaderUserId: null));
        await _departmentRepository.InsertAsync(new Department(id: Guid.Parse("28e385d1-ee7d-477f-949a-c94b2ef206d8"), code: "d93721fb195347deb9eb861744a0f69265d38f8c5041478f8e", name: "86a8a83da38249e9ad57858601a0d9503d44ed20cdb842028c", parentId: "cf9990bf5aac4becb34870", level: 40472334, sortOrder: 1940725416, isActive: true, leaderUserId: null));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}
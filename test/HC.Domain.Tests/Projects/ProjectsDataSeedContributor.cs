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
        await _projectRepository.InsertAsync(new Project(id: Guid.Parse("f89a627c-d03e-40d2-9e34-c9826b5a7d37"), code: "ec696583f9fe4cd4be70e7e19732b6ebb796a93ebf4a42e780", name: "a49f6e06c2c143198c08f12eb4615c48e351aeee13ba4c24b58bcb080dc48aef5701d60565994c75ac4078193382fa7365fefd72cb274789af7efee9e066d549b9abdc37ce8843e48f07181cf7f76abbd5794be630be42639359649cc2006129cf431b0733ab43689c4368571adda08f81ad7250b06345c8b1cf28fe5cb2dd1", description: "640fd0d1989640c0a2515489071941fb660ef09562d744fbb3380aff1cc2cc339d6", startDate: new DateTime(2022, 11, 3), endDate: new DateTime(2009, 9, 23), status: "74a5c59413864280a7f6cd79d2fe8ce47e949748d6814055b1a", ownerDepartmentId: null));
        await _projectRepository.InsertAsync(new Project(id: Guid.Parse("76264b58-2bbf-4ecc-b457-7a629b423e9c"), code: "ec2d49a9a4c541aab674bbe9f5da3c601b00588e404840d786", name: "182907e22fe641e093642dfa74e0c60b0272df59e7d14cc4ab1acebc188f41852c4a8e27e3f847cb934f021f48955268db1d744930de42de9073cac0eb8bb14414c3cb54a11a44e486baeb136b4a6918e45db9f4b1324eb78f2908bb6b4ea4bb41653b67250a49b3898c996b1cdc813e72fdde078b6b4a2c90582ba89236f86", description: "39eed5f3e9da4efeabc01da6f755ea80d2343bc5b5ed4e85a6f8632d3aab51af070ffc8c4f22404fb7d", startDate: new DateTime(2001, 4, 5), endDate: new DateTime(2001, 10, 22), status: "a3446a62a87a4a89a7125caf450cc96c2a6c51f7e8ca4caba622b858017cc8d65d6a9161035f40e28f4a4324c873f892d94", ownerDepartmentId: null));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}
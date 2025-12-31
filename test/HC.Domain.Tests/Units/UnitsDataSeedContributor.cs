using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.Units;

namespace HC.Units;

public class UnitsDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public UnitsDataSeedContributor(IUnitRepository unitRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _unitRepository = unitRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _unitRepository.InsertAsync(new Unit(id: Guid.Parse("25b91d47-2958-4b3c-8d97-23ffa17a5632"), code: "d06b26139659417a901f35173beb9ff32b0c5d1f5999463da7", name: "4141b3dabacf4d9c", sortOrder: 1325321624, isActive: true));
        await _unitRepository.InsertAsync(new Unit(id: Guid.Parse("26b63ee5-972f-43e8-8e7a-e59130deacc5"), code: "9d04e57ddc2d4320a248761de2ee50276ad4c0a891bb4d4795", name: "521b2d73e52840ce82a52ed801aebc1", sortOrder: 1465064805, isActive: true));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}
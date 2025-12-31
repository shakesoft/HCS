using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.MasterDatas;

namespace HC.MasterDatas;

public class MasterDatasDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public MasterDatasDataSeedContributor(IMasterDataRepository masterDataRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _masterDataRepository = masterDataRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _masterDataRepository.InsertAsync(new MasterData(id: Guid.Parse("12feb6c5-7d61-44a9-b5df-3e194308c1dc"), type: "8f0607d6a74d4d66a201ff2d492bdd6d293a4770d3e14eabb9", code: "837a777093ff47548bb9b6ed2efe0a2137f3b795c5714f8da6", name: "77b60d6806af4dbe9e78261c5dec0ff47", sortOrder: 6363, isActive: true));
        await _masterDataRepository.InsertAsync(new MasterData(id: Guid.Parse("e626b30d-fe62-43dc-ad24-ad0f2ea6d3cc"), type: "a889e92e75514f06ae98bc69be27b918ce79e9c4c22b48d3ad", code: "86377e023d5f49a890a57dd2d8cbae221fb39e4cae8644a7b1", name: "fcbcde00975b43909e5b7dc729501", sortOrder: 338, isActive: true));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}
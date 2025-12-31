using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using HC.Positions;

namespace HC.Positions;

public class PositionsDataSeedContributor : IDataSeedContributor, ISingletonDependency
{
    private bool IsSeeded = false;
    private readonly IPositionRepository _positionRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public PositionsDataSeedContributor(IPositionRepository positionRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _positionRepository = positionRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (IsSeeded)
        {
            return;
        }

        await _positionRepository.InsertAsync(new Position(id: Guid.Parse("34fcc9a7-223a-4ddb-8754-571ec41cc6ae"), code: "5f5a0638a6454cfc955d419883aab75c155dc1feced545eb92", name: "6f07005925ec4d57a4b64b0ddd3", signOrder: 57, isActive: true));
        await _positionRepository.InsertAsync(new Position(id: Guid.Parse("06d89d44-bd03-474b-aea8-dae068e6d17c"), code: "4252f1d0e4524747b4774e84ff1c1cf598ca7d171627422d8a", name: "d74f6bc846dc4d898f0300de260aa0eb02569f494a534a2b811dded8af31a6f5dab5bc6f66", signOrder: 78, isActive: true));
        await _unitOfWorkManager!.Current!.SaveChangesAsync();
        IsSeeded = true;
    }
}
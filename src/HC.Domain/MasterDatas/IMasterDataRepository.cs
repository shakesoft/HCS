using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.MasterDatas;

public partial interface IMasterDataRepository : IRepository<MasterData, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? type = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, CancellationToken cancellationToken = default);
    Task<List<MasterData>> GetListAsync(string? filterText = null, string? type = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? type = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, CancellationToken cancellationToken = default);
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.Units;

public partial interface IUnitRepository : IRepository<Unit, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, CancellationToken cancellationToken = default);
    Task<List<Unit>> GetListAsync(string? filterText = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, CancellationToken cancellationToken = default);
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.SurveyLocations;

public partial interface ISurveyLocationRepository : IRepository<SurveyLocation, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, CancellationToken cancellationToken = default);
    Task<List<SurveyLocation>> GetListAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? code = null, string? name = null, string? description = null, bool? isActive = null, CancellationToken cancellationToken = default);
}
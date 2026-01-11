using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.SurveyCriterias;

public partial interface ISurveyCriteriaRepository : IRepository<SurveyCriteria, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? code = null, string? name = null, string? image = null, int? displayOrderMin = null, int? displayOrderMax = null, bool? isActive = null, Guid? surveyLocationId = null, CancellationToken cancellationToken = default);
    Task<SurveyCriteriaWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<SurveyCriteriaWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? code = null, string? name = null, string? image = null, int? displayOrderMin = null, int? displayOrderMax = null, bool? isActive = null, Guid? surveyLocationId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<SurveyCriteria>> GetListAsync(string? filterText = null, string? code = null, string? name = null, string? image = null, int? displayOrderMin = null, int? displayOrderMax = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? code = null, string? name = null, string? image = null, int? displayOrderMin = null, int? displayOrderMax = null, bool? isActive = null, Guid? surveyLocationId = null, CancellationToken cancellationToken = default);
}
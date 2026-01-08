using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.UserSignatures;

public partial interface IUserSignatureRepository : IRepository<UserSignature, Guid>
{
    Task DeleteAllAsync(string? filterText = null, SignType? signType = null, string? providerCode = null, string? tokenRef = null, string? signatureImage = null, DateTime? validFromMin = null, DateTime? validFromMax = null, DateTime? validToMin = null, DateTime? validToMax = null, bool? isActive = null, Guid? identityUserId = null, CancellationToken cancellationToken = default);
    Task<UserSignatureWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<UserSignatureWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, SignType? signType = null, string? providerCode = null, string? tokenRef = null, string? signatureImage = null, DateTime? validFromMin = null, DateTime? validFromMax = null, DateTime? validToMin = null, DateTime? validToMax = null, bool? isActive = null, Guid? identityUserId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<UserSignature>> GetListAsync(string? filterText = null, SignType? signType = null, string? providerCode = null, string? tokenRef = null, string? signatureImage = null, DateTime? validFromMin = null, DateTime? validFromMax = null, DateTime? validToMin = null, DateTime? validToMax = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, SignType? signType = null, string? providerCode = null, string? tokenRef = null, string? signatureImage = null, DateTime? validFromMin = null, DateTime? validFromMax = null, DateTime? validToMin = null, DateTime? validToMax = null, bool? isActive = null, Guid? identityUserId = null, CancellationToken cancellationToken = default);
}
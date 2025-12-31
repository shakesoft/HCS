using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.DocumentFiles;

public partial interface IDocumentFileRepository : IRepository<DocumentFile, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? name = null, string? path = null, string? hash = null, bool? isSigned = null, DateTime? uploadedAtMin = null, DateTime? uploadedAtMax = null, Guid? documentId = null, CancellationToken cancellationToken = default);
    Task<DocumentFileWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<DocumentFileWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? name = null, string? path = null, string? hash = null, bool? isSigned = null, DateTime? uploadedAtMin = null, DateTime? uploadedAtMax = null, Guid? documentId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<DocumentFile>> GetListAsync(string? filterText = null, string? name = null, string? path = null, string? hash = null, bool? isSigned = null, DateTime? uploadedAtMin = null, DateTime? uploadedAtMax = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? name = null, string? path = null, string? hash = null, bool? isSigned = null, DateTime? uploadedAtMin = null, DateTime? uploadedAtMax = null, Guid? documentId = null, CancellationToken cancellationToken = default);
}
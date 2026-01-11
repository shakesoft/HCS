using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.SurveyFiles;

public partial interface ISurveyFileRepository : IRepository<SurveyFile, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? uploaderType = null, string? fileName = null, string? filePath = null, int? fileSizeMin = null, int? fileSizeMax = null, string? mimeType = null, string? fileType = null, Guid? surveySessionId = null, CancellationToken cancellationToken = default);
    Task<SurveyFileWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<SurveyFileWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? uploaderType = null, string? fileName = null, string? filePath = null, int? fileSizeMin = null, int? fileSizeMax = null, string? mimeType = null, string? fileType = null, Guid? surveySessionId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<SurveyFile>> GetListAsync(string? filterText = null, string? uploaderType = null, string? fileName = null, string? filePath = null, int? fileSizeMin = null, int? fileSizeMax = null, string? mimeType = null, string? fileType = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? uploaderType = null, string? fileName = null, string? filePath = null, int? fileSizeMin = null, int? fileSizeMax = null, string? mimeType = null, string? fileType = null, Guid? surveySessionId = null, CancellationToken cancellationToken = default);
}
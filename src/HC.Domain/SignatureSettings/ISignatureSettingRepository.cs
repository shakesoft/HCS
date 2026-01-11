using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.SignatureSettings;

public partial interface ISignatureSettingRepository : IRepository<SignatureSetting, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? providerCode = null, string? providerType = null, string? apiEndpoint = null, int? apiTimeoutMin = null, int? apiTimeoutMax = null, string? defaultSignType = null, bool? allowElectronicSign = null, bool? allowDigitalSign = null, bool? requireOtp = null, int? signWidthMin = null, int? signWidthMax = null, int? signHeightMin = null, int? signHeightMax = null, string? signedFileSuffix = null, bool? keepOriginalFile = null, bool? overwriteSignedFile = null, bool? enableSignLog = null, bool? isActive = null, CancellationToken cancellationToken = default);
    Task<List<SignatureSetting>> GetListAsync(string? filterText = null, string? providerCode = null, string? providerType = null, string? apiEndpoint = null, int? apiTimeoutMin = null, int? apiTimeoutMax = null, string? defaultSignType = null, bool? allowElectronicSign = null, bool? allowDigitalSign = null, bool? requireOtp = null, int? signWidthMin = null, int? signWidthMax = null, int? signHeightMin = null, int? signHeightMax = null, string? signedFileSuffix = null, bool? keepOriginalFile = null, bool? overwriteSignedFile = null, bool? enableSignLog = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? providerCode = null, string? providerType = null, string? apiEndpoint = null, int? apiTimeoutMin = null, int? apiTimeoutMax = null, string? defaultSignType = null, bool? allowElectronicSign = null, bool? allowDigitalSign = null, bool? requireOtp = null, int? signWidthMin = null, int? signWidthMax = null, int? signHeightMin = null, int? signHeightMax = null, string? signedFileSuffix = null, bool? keepOriginalFile = null, bool? overwriteSignedFile = null, bool? enableSignLog = null, bool? isActive = null, CancellationToken cancellationToken = default);
}
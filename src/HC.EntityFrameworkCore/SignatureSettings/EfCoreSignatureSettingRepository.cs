using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using HC.EntityFrameworkCore;

namespace HC.SignatureSettings;

public abstract class EfCoreSignatureSettingRepositoryBase : EfCoreRepository<HCDbContext, SignatureSetting, Guid>
{
    public EfCoreSignatureSettingRepositoryBase(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task DeleteAllAsync(string? filterText = null, string? providerCode = null, ProviderType? providerType = null, string? apiEndpoint = null, int? apiTimeoutMin = null, int? apiTimeoutMax = null, SignType? defaultSignType = null, bool? allowElectronicSign = null, bool? allowDigitalSign = null, bool? requireOtp = null, int? signWidthMin = null, int? signWidthMax = null, int? signHeightMin = null, int? signHeightMax = null, string? signedFileSuffix = null, bool? keepOriginalFile = null, bool? overwriteSignedFile = null, bool? enableSignLog = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = ApplyFilter(query, filterText, providerCode, providerType, apiEndpoint, apiTimeoutMin, apiTimeoutMax, defaultSignType, allowElectronicSign, allowDigitalSign, requireOtp, signWidthMin, signWidthMax, signHeightMin, signHeightMax, signedFileSuffix, keepOriginalFile, overwriteSignedFile, enableSignLog, isActive);
        var ids = query.Select(x => x.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<SignatureSetting>> GetListAsync(string? filterText = null, string? providerCode = null, ProviderType? providerType = null, string? apiEndpoint = null, int? apiTimeoutMin = null, int? apiTimeoutMax = null, SignType? defaultSignType = null, bool? allowElectronicSign = null, bool? allowDigitalSign = null, bool? requireOtp = null, int? signWidthMin = null, int? signWidthMax = null, int? signHeightMin = null, int? signHeightMax = null, string? signedFileSuffix = null, bool? keepOriginalFile = null, bool? overwriteSignedFile = null, bool? enableSignLog = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, providerCode, providerType, apiEndpoint, apiTimeoutMin, apiTimeoutMax, defaultSignType, allowElectronicSign, allowDigitalSign, requireOtp, signWidthMin, signWidthMax, signHeightMin, signHeightMax, signedFileSuffix, keepOriginalFile, overwriteSignedFile, enableSignLog, isActive);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SignatureSettingConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, string? providerCode = null, ProviderType? providerType = null, string? apiEndpoint = null, int? apiTimeoutMin = null, int? apiTimeoutMax = null, SignType? defaultSignType = null, bool? allowElectronicSign = null, bool? allowDigitalSign = null, bool? requireOtp = null, int? signWidthMin = null, int? signWidthMax = null, int? signHeightMin = null, int? signHeightMax = null, string? signedFileSuffix = null, bool? keepOriginalFile = null, bool? overwriteSignedFile = null, bool? enableSignLog = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, providerCode, providerType, apiEndpoint, apiTimeoutMin, apiTimeoutMax, defaultSignType, allowElectronicSign, allowDigitalSign, requireOtp, signWidthMin, signWidthMax, signHeightMin, signHeightMax, signedFileSuffix, keepOriginalFile, overwriteSignedFile, enableSignLog, isActive);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<SignatureSetting> ApplyFilter(IQueryable<SignatureSetting> query, string? filterText = null, string? providerCode = null, ProviderType? providerType = null, string? apiEndpoint = null, int? apiTimeoutMin = null, int? apiTimeoutMax = null, SignType? defaultSignType = null, bool? allowElectronicSign = null, bool? allowDigitalSign = null, bool? requireOtp = null, int? signWidthMin = null, int? signWidthMax = null, int? signHeightMin = null, int? signHeightMax = null, string? signedFileSuffix = null, bool? keepOriginalFile = null, bool? overwriteSignedFile = null, bool? enableSignLog = null, bool? isActive = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.ProviderCode!.Contains(filterText!) || e.ApiEndpoint!.Contains(filterText!) || e.SignedFileSuffix!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(providerCode), e => e.ProviderCode.Contains(providerCode)).WhereIf(providerType.HasValue, e => e.ProviderType == providerType).WhereIf(!string.IsNullOrWhiteSpace(apiEndpoint), e => e.ApiEndpoint.Contains(apiEndpoint)).WhereIf(apiTimeoutMin.HasValue, e => e.ApiTimeout >= apiTimeoutMin!.Value).WhereIf(apiTimeoutMax.HasValue, e => e.ApiTimeout <= apiTimeoutMax!.Value).WhereIf(defaultSignType.HasValue, e => e.DefaultSignType == defaultSignType).WhereIf(allowElectronicSign.HasValue, e => e.AllowElectronicSign == allowElectronicSign).WhereIf(allowDigitalSign.HasValue, e => e.AllowDigitalSign == allowDigitalSign).WhereIf(requireOtp.HasValue, e => e.RequireOtp == requireOtp).WhereIf(signWidthMin.HasValue, e => e.SignWidth >= signWidthMin!.Value).WhereIf(signWidthMax.HasValue, e => e.SignWidth <= signWidthMax!.Value).WhereIf(signHeightMin.HasValue, e => e.SignHeight >= signHeightMin!.Value).WhereIf(signHeightMax.HasValue, e => e.SignHeight <= signHeightMax!.Value).WhereIf(!string.IsNullOrWhiteSpace(signedFileSuffix), e => e.SignedFileSuffix.Contains(signedFileSuffix)).WhereIf(keepOriginalFile.HasValue, e => e.KeepOriginalFile == keepOriginalFile).WhereIf(overwriteSignedFile.HasValue, e => e.OverwriteSignedFile == overwriteSignedFile).WhereIf(enableSignLog.HasValue, e => e.EnableSignLog == enableSignLog).WhereIf(isActive.HasValue, e => e.IsActive == isActive);
    }
}
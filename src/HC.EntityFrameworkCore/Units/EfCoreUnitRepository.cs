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

namespace HC.Units;

public abstract class EfCoreUnitRepositoryBase : EfCoreRepository<HCDbContext, Unit, Guid>
{
    public EfCoreUnitRepositoryBase(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task DeleteAllAsync(string? filterText = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = ApplyFilter(query, filterText, code, name, sortOrderMin, sortOrderMax, isActive);
        var ids = query.Select(x => x.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Unit>> GetListAsync(string? filterText = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, code, name, sortOrderMin, sortOrderMax, isActive);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UnitConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, code, name, sortOrderMin, sortOrderMax, isActive);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Unit> ApplyFilter(IQueryable<Unit> query, string? filterText = null, string? code = null, string? name = null, int? sortOrderMin = null, int? sortOrderMax = null, bool? isActive = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Code!.Contains(filterText!) || e.Name!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code)).WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name)).WhereIf(sortOrderMin.HasValue, e => e.SortOrder >= sortOrderMin!.Value).WhereIf(sortOrderMax.HasValue, e => e.SortOrder <= sortOrderMax!.Value).WhereIf(isActive.HasValue, e => e.IsActive == isActive);
    }
}
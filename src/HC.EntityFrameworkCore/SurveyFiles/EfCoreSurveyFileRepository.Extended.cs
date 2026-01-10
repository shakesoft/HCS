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

namespace HC.SurveyFiles;

public class EfCoreSurveyFileRepository : EfCoreSurveyFileRepositoryBase, ISurveyFileRepository
{
    public EfCoreSurveyFileRepository(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
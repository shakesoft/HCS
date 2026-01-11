using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.SurveyResults;

public abstract class SurveyResultManagerBase : DomainService
{
    protected ISurveyResultRepository _surveyResultRepository;

    public SurveyResultManagerBase(ISurveyResultRepository surveyResultRepository)
    {
        _surveyResultRepository = surveyResultRepository;
    }

    public virtual async Task<SurveyResult> CreateAsync(Guid surveyCriteriaId, Guid surveySessionId, int rating)
    {
        Check.NotNull(surveyCriteriaId, nameof(surveyCriteriaId));
        Check.NotNull(surveySessionId, nameof(surveySessionId));
        var surveyResult = new SurveyResult(GuidGenerator.Create(), surveyCriteriaId, surveySessionId, rating);
        return await _surveyResultRepository.InsertAsync(surveyResult);
    }

    public virtual async Task<SurveyResult> UpdateAsync(Guid id, Guid surveyCriteriaId, Guid surveySessionId, int rating, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(surveyCriteriaId, nameof(surveyCriteriaId));
        Check.NotNull(surveySessionId, nameof(surveySessionId));
        var surveyResult = await _surveyResultRepository.GetAsync(id);
        surveyResult.SurveyCriteriaId = surveyCriteriaId;
        surveyResult.SurveySessionId = surveySessionId;
        surveyResult.Rating = rating;
        surveyResult.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _surveyResultRepository.UpdateAsync(surveyResult);
    }
}
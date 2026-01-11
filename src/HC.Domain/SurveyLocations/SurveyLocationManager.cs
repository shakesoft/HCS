using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.SurveyLocations;

public abstract class SurveyLocationManagerBase : DomainService
{
    protected ISurveyLocationRepository _surveyLocationRepository;

    public SurveyLocationManagerBase(ISurveyLocationRepository surveyLocationRepository)
    {
        _surveyLocationRepository = surveyLocationRepository;
    }

    public virtual async Task<SurveyLocation> CreateAsync(string code, string name, bool isActive, string? description = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var surveyLocation = new SurveyLocation(GuidGenerator.Create(), code, name, isActive, description);
        return await _surveyLocationRepository.InsertAsync(surveyLocation);
    }

    public virtual async Task<SurveyLocation> UpdateAsync(Guid id, string code, string name, bool isActive, string? description = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var surveyLocation = await _surveyLocationRepository.GetAsync(id);
        surveyLocation.Code = code;
        surveyLocation.Name = name;
        surveyLocation.IsActive = isActive;
        surveyLocation.Description = description;
        surveyLocation.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _surveyLocationRepository.UpdateAsync(surveyLocation);
    }
}
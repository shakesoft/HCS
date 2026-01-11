using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.SurveyCriterias;

public abstract class SurveyCriteriaManagerBase : DomainService
{
    protected ISurveyCriteriaRepository _surveyCriteriaRepository;

    public SurveyCriteriaManagerBase(ISurveyCriteriaRepository surveyCriteriaRepository)
    {
        _surveyCriteriaRepository = surveyCriteriaRepository;
    }

    public virtual async Task<SurveyCriteria> CreateAsync(Guid surveyLocationId, string code, string name, string image, int displayOrder, bool isActive)
    {
        Check.NotNull(surveyLocationId, nameof(surveyLocationId));
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNullOrWhiteSpace(image, nameof(image));
        var surveyCriteria = new SurveyCriteria(GuidGenerator.Create(), surveyLocationId, code, name, image, displayOrder, isActive);
        return await _surveyCriteriaRepository.InsertAsync(surveyCriteria);
    }

    public virtual async Task<SurveyCriteria> UpdateAsync(Guid id, Guid surveyLocationId, string code, string name, string image, int displayOrder, bool isActive, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(surveyLocationId, nameof(surveyLocationId));
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNullOrWhiteSpace(image, nameof(image));
        var surveyCriteria = await _surveyCriteriaRepository.GetAsync(id);
        surveyCriteria.SurveyLocationId = surveyLocationId;
        surveyCriteria.Code = code;
        surveyCriteria.Name = name;
        surveyCriteria.Image = image;
        surveyCriteria.DisplayOrder = displayOrder;
        surveyCriteria.IsActive = isActive;
        surveyCriteria.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _surveyCriteriaRepository.UpdateAsync(surveyCriteria);
    }
}
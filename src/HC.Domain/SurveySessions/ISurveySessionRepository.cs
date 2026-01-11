using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.SurveySessions;

public partial interface ISurveySessionRepository : IRepository<SurveySession, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? fullName = null, string? phoneNumber = null, string? patientCode = null, DateTime? surveyTimeMin = null, DateTime? surveyTimeMax = null, string? deviceType = null, string? note = null, string? sessionDisplay = null, Guid? surveyLocationId = null, CancellationToken cancellationToken = default);
    Task<SurveySessionWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<SurveySessionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? fullName = null, string? phoneNumber = null, string? patientCode = null, DateTime? surveyTimeMin = null, DateTime? surveyTimeMax = null, string? deviceType = null, string? note = null, string? sessionDisplay = null, Guid? surveyLocationId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<SurveySession>> GetListAsync(string? filterText = null, string? fullName = null, string? phoneNumber = null, string? patientCode = null, DateTime? surveyTimeMin = null, DateTime? surveyTimeMax = null, string? deviceType = null, string? note = null, string? sessionDisplay = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? fullName = null, string? phoneNumber = null, string? patientCode = null, DateTime? surveyTimeMin = null, DateTime? surveyTimeMax = null, string? deviceType = null, string? note = null, string? sessionDisplay = null, Guid? surveyLocationId = null, CancellationToken cancellationToken = default);
}
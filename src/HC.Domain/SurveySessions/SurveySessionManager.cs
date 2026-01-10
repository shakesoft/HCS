using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.SurveySessions;

public abstract class SurveySessionManagerBase : DomainService
{
    protected ISurveySessionRepository _surveySessionRepository;

    public SurveySessionManagerBase(ISurveySessionRepository surveySessionRepository)
    {
        _surveySessionRepository = surveySessionRepository;
    }

    public virtual async Task<SurveySession> CreateAsync(Guid surveyLocationId, DateTime surveyTime, string sessionDisplay, string? fullName = null, string? phoneNumber = null, string? patientCode = null, string? deviceType = null, string? note = null)
    {
        Check.NotNull(surveyLocationId, nameof(surveyLocationId));
        Check.NotNull(surveyTime, nameof(surveyTime));
        Check.NotNullOrWhiteSpace(sessionDisplay, nameof(sessionDisplay));
        var surveySession = new SurveySession(GuidGenerator.Create(), surveyLocationId, surveyTime, sessionDisplay, fullName, phoneNumber, patientCode, deviceType, note);
        return await _surveySessionRepository.InsertAsync(surveySession);
    }

    public virtual async Task<SurveySession> UpdateAsync(Guid id, Guid surveyLocationId, DateTime surveyTime, string sessionDisplay, string? fullName = null, string? phoneNumber = null, string? patientCode = null, string? deviceType = null, string? note = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(surveyLocationId, nameof(surveyLocationId));
        Check.NotNull(surveyTime, nameof(surveyTime));
        Check.NotNullOrWhiteSpace(sessionDisplay, nameof(sessionDisplay));
        var surveySession = await _surveySessionRepository.GetAsync(id);
        surveySession.SurveyLocationId = surveyLocationId;
        surveySession.SurveyTime = surveyTime;
        surveySession.SessionDisplay = sessionDisplay;
        surveySession.FullName = fullName;
        surveySession.PhoneNumber = phoneNumber;
        surveySession.PatientCode = patientCode;
        surveySession.DeviceType = deviceType;
        surveySession.Note = note;
        surveySession.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _surveySessionRepository.UpdateAsync(surveySession);
    }
}
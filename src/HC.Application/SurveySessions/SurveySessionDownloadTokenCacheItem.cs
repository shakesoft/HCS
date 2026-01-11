using System;

namespace HC.SurveySessions;

public abstract class SurveySessionDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}
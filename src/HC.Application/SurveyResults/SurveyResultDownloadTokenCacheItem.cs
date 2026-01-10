using System;

namespace HC.SurveyResults;

public abstract class SurveyResultDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}
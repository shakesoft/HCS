using System;

namespace HC.SurveyLocations;

public abstract class SurveyLocationDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}
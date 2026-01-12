using System;

namespace HC.MasterDatas;

/// <summary>
/// Enum for MasterData Type values
/// </summary>
public enum MasterDataType
{
    /// <summary>
    /// Loại văn bản
    /// </summary>
    DocumentType = 1,

    /// <summary>
    /// Mức độ khẩn
    /// </summary>
    UrgencyLevel = 2,

    /// <summary>
    /// Mức độ mật
    /// </summary>
    SecrecyLevel = 3,

    /// <summary>
    /// Lĩnh vực văn bản
    /// </summary>
    Field = 4,

    /// <summary>
    /// Trạng thái văn bản
    /// </summary>
    Status = 5
}

/// <summary>
/// Extension methods for MasterDataType enum
/// </summary>
public static class MasterDataTypeExtensions
{
    /// <summary>
    /// Get the string value for MasterData Type field
    /// </summary>
    public static string GetTypeValue(this MasterDataType type)
    {
        return type switch
        {
            MasterDataType.DocumentType => "LOAI_VB",
            MasterDataType.UrgencyLevel => "MUC_DO_KHAN",
            MasterDataType.SecrecyLevel => "MUC_DO_MAT",
            MasterDataType.Field => "LINH_VUC_VB",
            MasterDataType.Status => "TRANG_THAI_VB",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown MasterDataType")
        };
    }
}

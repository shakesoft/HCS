using HC.Blazor;

namespace HC.Blazor.Shared;

/// <summary>
/// Base class for pages that need validation support
/// Provides Create and Edit validation helpers
/// </summary>
public abstract class ValidationPageBase : HCComponentBase
{
    /// <summary>
    /// Validation helper for Create mode
    /// </summary>
    protected ValidationHelper CreateValidation { get; } = new();

    /// <summary>
    /// Validation helper for Edit mode
    /// </summary>
    protected ValidationHelper EditValidation { get; } = new();

    /// <summary>
    /// Get error message for a field in Create mode
    /// </summary>
    protected string? GetCreateFieldError(string fieldName) => CreateValidation.GetFieldError(fieldName);

    /// <summary>
    /// Get error message for a field in Edit mode
    /// </summary>
    protected string? GetEditFieldError(string fieldName) => EditValidation.GetFieldError(fieldName);

    /// <summary>
    /// Check if a field has error in Create mode
    /// </summary>
    protected bool HasCreateFieldError(string fieldName) => CreateValidation.HasFieldError(fieldName);

    /// <summary>
    /// Check if a field has error in Edit mode
    /// </summary>
    protected bool HasEditFieldError(string fieldName) => EditValidation.HasFieldError(fieldName);

    /// <summary>
    /// Get first validation error key for Create mode
    /// </summary>
    protected string? CreateValidationErrorKey => CreateValidation.FirstValidationErrorKey;

    /// <summary>
    /// Get first validation error key for Edit mode
    /// </summary>
    protected string? EditValidationErrorKey => EditValidation.FirstValidationErrorKey;

    /// <summary>
    /// Remove error for a field in Create mode (useful in TextChanged handlers)
    /// </summary>
    protected void RemoveCreateFieldError(string fieldName) => CreateValidation.RemoveFieldError(fieldName);

    /// <summary>
    /// Remove error for a field in Edit mode (useful in TextChanged handlers)
    /// </summary>
    protected void RemoveEditFieldError(string fieldName) => EditValidation.RemoveFieldError(fieldName);
}

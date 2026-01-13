using System;
using System.Collections.Generic;
using System.Linq;

namespace HC.Blazor.Shared;

/// <summary>
/// Dynamic validation helper for Blazor pages to reduce code duplication
/// </summary>
public class ValidationHelper
{
    private readonly Dictionary<string, string?> _fieldErrors = new();
    private string? _firstValidationErrorKey;

    /// <summary>
    /// Get error message for a field
    /// </summary>
    public string? GetFieldError(string fieldName) => _fieldErrors.GetValueOrDefault(fieldName);

    /// <summary>
    /// Check if a field has error
    /// </summary>
    public bool HasFieldError(string fieldName) => 
        _fieldErrors.ContainsKey(fieldName) && !string.IsNullOrWhiteSpace(_fieldErrors[fieldName]);

    /// <summary>
    /// Get the first validation error key (for displaying general error message)
    /// </summary>
    public string? FirstValidationErrorKey => _firstValidationErrorKey;

    /// <summary>
    /// Check if validation passed
    /// </summary>
    public bool IsValid => _fieldErrors.Count == 0;

    /// <summary>
    /// Get all field errors
    /// </summary>
    public IReadOnlyDictionary<string, string?> FieldErrors => _fieldErrors;

    /// <summary>
    /// Reset validation state
    /// </summary>
    public void Reset()
    {
        _fieldErrors.Clear();
        _firstValidationErrorKey = null;
    }

    /// <summary>
    /// Remove error for a specific field
    /// </summary>
    public void RemoveFieldError(string fieldName)
    {
        _fieldErrors.Remove(fieldName);
    }

    /// <summary>
    /// Validate required string field
    /// </summary>
    public bool ValidateRequiredString(string fieldName, string? value, string errorKey, Func<string> getErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            SetFieldError(fieldName, errorKey, getErrorMessage());
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate required Guid field (must not be empty)
    /// </summary>
    public bool ValidateRequiredGuid(string fieldName, Guid value, string errorKey, Func<string> getErrorMessage)
    {
        if (value == Guid.Empty)
        {
            SetFieldError(fieldName, errorKey, getErrorMessage());
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate required nullable Guid field
    /// </summary>
    public bool ValidateRequiredGuid(string fieldName, Guid? value, string errorKey, Func<string> getErrorMessage)
    {
        if (!value.HasValue || value.Value == Guid.Empty)
        {
            SetFieldError(fieldName, errorKey, getErrorMessage());
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate required list/collection field
    /// </summary>
    public bool ValidateRequiredCollection<T>(string fieldName, ICollection<T>? value, string errorKey, Func<string> getErrorMessage)
    {
        if (value == null || value.Count == 0)
        {
            SetFieldError(fieldName, errorKey, getErrorMessage());
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate range for numeric value
    /// </summary>
    public bool ValidateRange(string fieldName, int value, int min, int max, string errorKey, Func<string> getErrorMessage)
    {
        if (value < min || value > max)
        {
            SetFieldError(fieldName, errorKey, getErrorMessage());
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate range for decimal value
    /// </summary>
    public bool ValidateRange(string fieldName, decimal value, decimal min, decimal max, string errorKey, Func<string> getErrorMessage)
    {
        if (value < min || value > max)
        {
            SetFieldError(fieldName, errorKey, getErrorMessage());
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate custom rule
    /// </summary>
    public bool ValidateCustom(string fieldName, bool isValid, string errorKey, Func<string> getErrorMessage)
    {
        if (!isValid)
        {
            SetFieldError(fieldName, errorKey, getErrorMessage());
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate with custom predicate
    /// </summary>
    public bool ValidateIf<T>(string fieldName, T value, Func<T, bool> predicate, string errorKey, Func<string> getErrorMessage)
    {
        if (!predicate(value))
        {
            SetFieldError(fieldName, errorKey, getErrorMessage());
            return false;
        }
        return true;
    }

    /// <summary>
    /// Set field error and track first error key
    /// </summary>
    private void SetFieldError(string fieldName, string errorKey, string errorMessage)
    {
        _fieldErrors[fieldName] = errorMessage;
        
        // Set first error key only if not already set
        if (string.IsNullOrWhiteSpace(_firstValidationErrorKey))
        {
            _firstValidationErrorKey = errorKey;
        }
    }
}

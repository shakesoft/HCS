# Validation Helper

Validation helper được tạo để giảm code lặp lại trong các Blazor pages khi thực hiện validation.

## Cách sử dụng

### 1. Sử dụng ValidationHelper trực tiếp

```csharp
using HC.Blazor.Shared;

public partial class MyPage : HCComponentBase
{
    // Tạo instance cho Create và Edit mode
    private ValidationHelper CreateValidation { get; } = new();
    private ValidationHelper EditValidation { get; } = new();
    
    // Helper methods (tùy chọn, để tương thích với Razor markup)
    private string? GetCreateFieldError(string fieldName) => CreateValidation.GetFieldError(fieldName);
    private bool HasCreateFieldError(string fieldName) => CreateValidation.HasFieldError(fieldName);
    
    // Validation method
    private bool ValidateCreate()
    {
        CreateValidation.Reset();
        
        // Validate required string
        CreateValidation.ValidateRequiredString("Name", MyDto.Name, "NameRequired", () => L["NameRequired"]);
        
        // Validate required Guid
        CreateValidation.ValidateRequiredGuid("ProjectId", MyDto.ProjectId, "ProjectRequired", () => L["ProjectRequired"]);
        
        // Validate required collection
        CreateValidation.ValidateRequiredCollection("Members", SelectedMembers, "MembersRequired", () => L["MembersRequired"]);
        
        // Validate range
        CreateValidation.ValidateRange("Progress", MyDto.ProgressPercent, 0, 100, "ProgressRange", () => L["ProgressRange"]);
        
        // Custom validation
        CreateValidation.ValidateCustom("CustomField", MyDto.SomeCondition, "CustomError", () => L["CustomError"]);
        
        return CreateValidation.IsValid;
    }
}
```

### 2. Sử dụng ValidationPageBase (Recommended)

```csharp
using HC.Blazor.Shared;

public partial class MyPage : ValidationPageBase
{
    // ValidationPageBase đã cung cấp sẵn:
    // - CreateValidation
    // - EditValidation
    // - GetCreateFieldError()
    // - GetEditFieldError()
    // - HasCreateFieldError()
    // - HasEditFieldError()
    // - CreateValidationErrorKey
    // - EditValidationErrorKey
    // - RemoveCreateFieldError()
    // - RemoveEditFieldError()
    
    private bool ValidateCreate()
    {
        CreateValidation.Reset();
        
        CreateValidation.ValidateRequiredString("Name", MyDto.Name, "NameRequired", () => L["NameRequired"]);
        CreateValidation.ValidateRequiredGuid("ProjectId", MyDto.ProjectId, "ProjectRequired", () => L["ProjectRequired"]);
        
        return CreateValidation.IsValid;
    }
}
```

### 3. Trong Razor markup

```razor
<Field>
    <FieldLabel>@L["Name"] *</FieldLabel>
    <TextEdit Text="@MyDto.Name" 
              TextChanged="@((string value) => { MyDto.Name = value; CreateValidation.RemoveFieldError("Name"); })"
              Style="@(HasCreateFieldError("Name") ? "border-color: #dc3545;" : "")" />
    @if (HasCreateFieldError("Name"))
    {
        <div class="invalid-feedback d-block">@GetCreateFieldError("Name")</div>
    }
</Field>
```

## Các phương thức validation có sẵn

- `ValidateRequiredString()` - Validate string không được null hoặc empty
- `ValidateRequiredGuid()` - Validate Guid không được Empty
- `ValidateRequiredCollection<T>()` - Validate collection không được null hoặc empty
- `ValidateRange()` - Validate giá trị số trong khoảng min-max
- `ValidateCustom()` - Validate với điều kiện tùy chỉnh
- `ValidateIf<T>()` - Validate với predicate function

## Ví dụ refactor từ code cũ

**Trước:**
```csharp
private bool ValidateCreateDocument()
{
    CreateDocumentValidationErrorKey = null;
    CreateFieldErrors.Clear();
    bool isValid = true;
    
    if (string.IsNullOrWhiteSpace(DocumentCreateData?.StorageNumber))
    {
        CreateFieldErrors["StorageNumber"] = L["StorageNumberRequired"];
        CreateDocumentValidationErrorKey = "StorageNumberRequired";
        isValid = false;
    }
    
    if (string.IsNullOrWhiteSpace(DocumentCreateData?.Title))
    {
        CreateFieldErrors["Title"] = L["TitleRequired"];
        if (isValid)
        {
            CreateDocumentValidationErrorKey = "TitleRequired";
        }
        isValid = false;
    }
    
    return isValid;
}
```

**Sau:**
```csharp
private bool ValidateCreateDocument()
{
    CreateValidation.Reset();
    
    CreateValidation.ValidateRequiredString("StorageNumber", DocumentCreateData?.StorageNumber, "StorageNumberRequired", () => L["StorageNumberRequired"]);
    CreateValidation.ValidateRequiredString("Title", DocumentCreateData?.Title, "TitleRequired", () => L["TitleRequired"]);
    
    return CreateValidation.IsValid;
}
```

## Lợi ích

1. **Giảm code lặp lại**: Không cần viết lại logic validation cho mỗi field
2. **Dễ bảo trì**: Thay đổi logic validation ở một nơi, áp dụng cho tất cả
3. **Nhất quán**: Tất cả pages sử dụng cùng một cách validation
4. **Clean code**: Code ngắn gọn và dễ đọc hơn

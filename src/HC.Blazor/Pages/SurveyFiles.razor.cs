using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using HC.SurveyFiles;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;
using Volo.Abp.BlobStoring;

namespace HC.Blazor.Pages;

public partial class SurveyFiles
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<SurveyFileWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<SurveyFileWithNavigationPropertiesDto> SurveyFileList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateSurveyFile { get; set; }

    private bool CanEditSurveyFile { get; set; }

    private bool CanDeleteSurveyFile { get; set; }

    private SurveyFileCreateDto NewSurveyFile { get; set; }

    private Validations NewSurveyFileValidations { get; set; } = new();
    private SurveyFileUpdateDto EditingSurveyFile { get; set; }

    private Validations EditingSurveyFileValidations { get; set; } = new();
    private Guid EditingSurveyFileId { get; set; }

    private Modal CreateSurveyFileModal { get; set; } = new();
    private Modal EditSurveyFileModal { get; set; } = new();
    private GetSurveyFilesInput Filter { get; set; }

    private DataGridEntityActionsColumn<SurveyFileWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "surveyFile-create-tab";
    protected string SelectedEditTab = "surveyFile-edit-tab";
    private SurveyFileWithNavigationPropertiesDto? SelectedSurveyFile;

    private IReadOnlyList<LookupDto<Guid>> SurveySessionsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<SurveyFileWithNavigationPropertiesDto> SelectedSurveyFiles { get; set; } = new();
    private bool AllSurveyFilesSelected { get; set; }

    // File upload (shared between Create and Edit modes)
    private FilePicker CreateFilePicker { get; set; } = new();
    private FilePicker EditFilePicker { get; set; } = new();
    private IFileEntry? SelectedFile { get; set; }
    private string UploadedFilePath { get; set; } = string.Empty;
    private bool IsUploadingFile { get; set; }
    private int FilePickerProgress { get; set; }

    [Inject]
    private IBlobContainer BlobContainer { get; set; } = default!;

    public SurveyFiles()
    {
        NewSurveyFile = new SurveyFileCreateDto();
        EditingSurveyFile = new SurveyFileUpdateDto();
        Filter = new GetSurveyFilesInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        SurveyFileList = new List<SurveyFileWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetSurveySessionCollectionLookupAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["SurveyFiles"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewSurveyFile"], async () => {
            await OpenCreateSurveyFileModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.SurveyFiles.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(SurveyFileWithNavigationPropertiesDto surveyFile)
    {
        DataGridRef.ToggleDetailRow(surveyFile, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<SurveyFileWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteSurveyFile;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<SurveyFileWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateSurveyFile = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyFiles.Create);
        CanEditSurveyFile = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyFiles.Edit);
        CanDeleteSurveyFile = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyFiles.Delete);
    }

    private async Task GetSurveyFilesAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await SurveyFilesAppService.GetListAsync(Filter);
        SurveyFileList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetSurveyFilesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await SurveyFilesAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/survey-files/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&UploaderType={HttpUtility.UrlEncode(Filter.UploaderType?.ToString())}&FileName={HttpUtility.UrlEncode(Filter.FileName)}&FilePath={HttpUtility.UrlEncode(Filter.FilePath)}&FileSizeMin={Filter.FileSizeMin}&FileSizeMax={Filter.FileSizeMax}&MimeType={HttpUtility.UrlEncode(Filter.MimeType)}&FileType={HttpUtility.UrlEncode(Filter.FileType)}&SurveySessionId={Filter.SurveySessionId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SurveyFileWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetSurveyFilesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateSurveyFileModalAsync()
    {
        NewSurveyFile = new SurveyFileCreateDto
        {
            SurveySessionId = SurveySessionsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        SelectedCreateTab = "surveyFile-create-tab";

        // Reset file upload state
        ResetFileUploadState();

        await NewSurveyFileValidations.ClearAll();
        await CreateSurveyFileModal.Show();
    }

    private async Task CloseCreateSurveyFileModalAsync()
    {
        NewSurveyFile = new SurveyFileCreateDto
        {
            SurveySessionId = SurveySessionsCollection.Select(i => i.Id).FirstOrDefault(),
        };

        // Reset file upload state
        ResetFileUploadState();

        await CreateSurveyFileModal.Hide();
    }

    private async Task OpenEditSurveyFileModalAsync(SurveyFileWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "surveyFile-edit-tab";
        var surveyFile = await SurveyFilesAppService.GetWithNavigationPropertiesAsync(input.SurveyFile.Id);
        EditingSurveyFileId = surveyFile.SurveyFile.Id;
        EditingSurveyFile = ObjectMapper.Map<SurveyFileDto, SurveyFileUpdateDto>(surveyFile.SurveyFile);

        // Reset file upload state
        ResetFileUploadState();

        await EditingSurveyFileValidations.ClearAll();
        await EditSurveyFileModal.Show();
    }

    private async Task DeleteSurveyFileAsync(SurveyFileWithNavigationPropertiesDto input)
    {
        try
        {
            await SurveyFilesAppService.DeleteAsync(input.SurveyFile.Id);
            await GetSurveyFilesAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task DeleteSurveyFileWithConfirmationAsync(SurveyFileWithNavigationPropertiesDto input)
    {
        if (await UiMessageService.Confirm(L["DeleteConfirmationMessage"].Value))
        {
            await DeleteSurveyFileAsync(input);
        }
    }

    private async Task CreateSurveyFileAsync()
    {
        try
        {
            if (await NewSurveyFileValidations.ValidateAll() == false)
            {
                return;
            }

            await SurveyFilesAppService.CreateAsync(NewSurveyFile);
            await GetSurveyFilesAsync();
            await CloseCreateSurveyFileModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditSurveyFileModalAsync()
    {
        // Reset file upload state
        ResetFileUploadState();

        await EditSurveyFileModal.Hide();
    }

    private async Task UpdateSurveyFileAsync()
    {
        try
        {
            if (await EditingSurveyFileValidations.ValidateAll() == false)
            {
                return;
            }

            await SurveyFilesAppService.UpdateAsync(EditingSurveyFileId, EditingSurveyFile);
            await GetSurveyFilesAsync();
            await EditSurveyFileModal.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void OnSelectedCreateTabChanged(string name)
    {
        SelectedCreateTab = name;
    }

    private void OnSelectedEditTabChanged(string name)
    {
        SelectedEditTab = name;
    }

    protected virtual async Task OnUploaderTypeChangedAsync(UploaderType? uploaderType)
    {
        Filter.UploaderType = uploaderType;
        await SearchAsync();
    }

    protected virtual async Task OnFileNameChangedAsync(string? fileName)
    {
        Filter.FileName = fileName;
        await SearchAsync();
    }

    protected virtual async Task OnFilePathChangedAsync(string? filePath)
    {
        Filter.FilePath = filePath;
        await SearchAsync();
    }

    protected virtual async Task OnFileSizeMinChangedAsync(int? fileSizeMin)
    {
        Filter.FileSizeMin = fileSizeMin;
        await SearchAsync();
    }

    protected virtual async Task OnFileSizeMaxChangedAsync(int? fileSizeMax)
    {
        Filter.FileSizeMax = fileSizeMax;
        await SearchAsync();
    }

    protected virtual async Task OnMimeTypeChangedAsync(string? mimeType)
    {
        Filter.MimeType = mimeType;
        await SearchAsync();
    }

    protected virtual async Task OnFileTypeChangedAsync(string? fileType)
    {
        Filter.FileType = fileType;
        await SearchAsync();
    }

    protected virtual async Task OnSurveySessionIdChangedAsync(Guid? surveySessionId)
    {
        Filter.SurveySessionId = surveySessionId;
        await SearchAsync();
    }

    private async Task GetSurveySessionCollectionLookupAsync(string? newValue = null)
    {
        SurveySessionsCollection = (await SurveyFilesAppService.GetSurveySessionLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllSurveyFilesSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllSurveyFilesSelected = false;
        SelectedSurveyFiles.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedSurveyFileRowsChanged()
    {
        if (SelectedSurveyFiles.Count != PageSize)
        {
            AllSurveyFilesSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedSurveyFilesAsync()
    {
        var message = AllSurveyFilesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedSurveyFiles.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllSurveyFilesSelected)
        {
            await SurveyFilesAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await SurveyFilesAppService.DeleteByIdsAsync(SelectedSurveyFiles.Select(x => x.SurveyFile.Id).ToList());
        }

        SelectedSurveyFiles.Clear();
        AllSurveyFilesSelected = false;
        await GetSurveyFilesAsync();
    }

    // File upload handler for Create mode
    private async Task OnCreateFileChanged(FileChangedEventArgs e)
    {
        if (e.Files != null && e.Files.Any())
        {
            var file = e.Files.First();
            await UploadFileAsync(file, isEditMode: false);
        }
    }

    // File upload handler for Edit mode
    private async Task OnEditFileChanged(FileChangedEventArgs e)
    {
        if (e.Files != null && e.Files.Any())
        {
            var file = e.Files.First();
            await UploadFileAsync(file, isEditMode: true);
        }
    }

    // Common method for uploading files
    private async Task UploadFileAsync(IFileEntry file, bool isEditMode)
    {
        try
        {
            // Validate file size (50MB max)
            if (file.Size > 52428800)
            {
                await UiMessageService.Error(L["FileSizeTooLarge", 50]);
                // Clear the file picker
                if (isEditMode)
                {
                    await EditFilePicker.Clear();
                }
                else
                {
                    await CreateFilePicker.Clear();
                }
                return;
            }

            // Set uploading state
            IsUploadingFile = true;
            SelectedFile = file;
            FilePickerProgress = 0;

            // Read file content
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream(long.MaxValue).CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var fileBytes = memoryStream.ToArray();

            // Generate unique file name
            var fileName = $"{Guid.NewGuid()}_{file.Name}";
            var filePath = $"survey-files/{fileName}";

            // Upload to MinIO
            await BlobContainer.SaveAsync(filePath, fileBytes);

            // Get file extension and MIME type
            var fileExtension = Path.GetExtension(file.Name).TrimStart('.');
            var mimeType = file.Type;

            // Auto-fill form fields
            UploadedFilePath = filePath;
            if (isEditMode)
            {
                EditingSurveyFile.FileName = file.Name;
                EditingSurveyFile.FilePath = filePath;
                EditingSurveyFile.FileSize = (int)file.Size;
                EditingSurveyFile.MimeType = mimeType;
                EditingSurveyFile.FileType = fileExtension;
            }
            else
            {
                NewSurveyFile.FileName = file.Name;
                NewSurveyFile.FilePath = filePath;
                NewSurveyFile.FileSize = (int)file.Size;
                NewSurveyFile.MimeType = mimeType;
                NewSurveyFile.FileType = fileExtension;
            }
            FilePickerProgress = 100;

            await UiMessageService.Success(L["FileUploadedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            ResetFileUploadState();
        }
        finally
        {
            IsUploadingFile = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    // Helper method to reset file upload state
    private void ResetFileUploadState()
    {
        SelectedFile = null;
        UploadedFilePath = string.Empty;
        FilePickerProgress = 0;
        IsUploadingFile = false;
    }
}
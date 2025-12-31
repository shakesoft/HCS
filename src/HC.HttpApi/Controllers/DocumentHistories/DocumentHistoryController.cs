using HC.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.DocumentHistories;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Controllers.DocumentHistories;

[RemoteService]
[Area("app")]
[ControllerName("DocumentHistory")]
[Route("api/app/document-histories")]
public abstract class DocumentHistoryControllerBase : AbpController
{
    protected IDocumentHistoriesAppService _documentHistoriesAppService;

    public DocumentHistoryControllerBase(IDocumentHistoriesAppService documentHistoriesAppService)
    {
        _documentHistoriesAppService = documentHistoriesAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<DocumentHistoryWithNavigationPropertiesDto>> GetListAsync(GetDocumentHistoriesInput input)
    {
        return _documentHistoriesAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("with-navigation-properties/{id}")]
    public virtual Task<DocumentHistoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
    {
        return _documentHistoriesAppService.GetWithNavigationPropertiesAsync(id);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<DocumentHistoryDto> GetAsync(Guid id)
    {
        return _documentHistoriesAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("document-lookup")]
    public virtual Task<PagedResultDto<LookupDto<Guid>>> GetDocumentLookupAsync(LookupRequestDto input)
    {
        return _documentHistoriesAppService.GetDocumentLookupAsync(input);
    }

    [HttpGet]
    [Route("identity-user-lookup")]
    public virtual Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input)
    {
        return _documentHistoriesAppService.GetIdentityUserLookupAsync(input);
    }

    [HttpPost]
    public virtual Task<DocumentHistoryDto> CreateAsync(DocumentHistoryCreateDto input)
    {
        return _documentHistoriesAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<DocumentHistoryDto> UpdateAsync(Guid id, DocumentHistoryUpdateDto input)
    {
        return _documentHistoriesAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _documentHistoriesAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(DocumentHistoryExcelDownloadDto input)
    {
        return _documentHistoriesAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return _documentHistoriesAppService.GetDownloadTokenAsync();
    }

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> documenthistoryIds)
    {
        return _documentHistoriesAppService.DeleteByIdsAsync(documenthistoryIds);
    }

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetDocumentHistoriesInput input)
    {
        return _documentHistoriesAppService.DeleteAllAsync(input);
    }
}
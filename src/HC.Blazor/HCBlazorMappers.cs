using HC.Notifications;
using HC.ProjectTaskDocuments;
using HC.ProjectTaskAssignments;
using HC.ProjectTasks;
using HC.ProjectMembers;
using HC.Projects;
using HC.DocumentHistories;
using HC.DocumentAssignments;
using HC.DocumentWorkflowInstances;
using HC.DocumentFiles;
using HC.Documents;
using HC.WorkflowStepAssignments;
using HC.Units;
using HC.Departments;
using HC.WorkflowStepTemplates;
using HC.WorkflowTemplates;
using HC.Workflows;
using HC.WorkflowDefinitions;
using HC.MasterDatas;
using HC.Positions;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using HC.Books;

namespace HC.Blazor;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class HCBlazorMappers : MapperBase<BookDto, CreateUpdateBookDto>
{
    public override partial CreateUpdateBookDto Map(BookDto source);
    public override partial void Map(BookDto source, CreateUpdateBookDto destination);
}

[Mapper]
public partial class PositionDtoToPositionUpdateDtoBlazorMapper : MapperBase<PositionDto, PositionUpdateDto>
{
    public override partial PositionUpdateDto Map(PositionDto source);
    public override partial void Map(PositionDto source, PositionUpdateDto destination);
}

[Mapper]
public partial class MasterDataDtoToMasterDataUpdateDtoBlazorMapper : MapperBase<MasterDataDto, MasterDataUpdateDto>
{
    public override partial MasterDataUpdateDto Map(MasterDataDto source);
    public override partial void Map(MasterDataDto source, MasterDataUpdateDto destination);
}

[Mapper]
public partial class WorkflowDefinitionDtoToWorkflowDefinitionUpdateDtoBlazorMapper : MapperBase<WorkflowDefinitionDto, WorkflowDefinitionUpdateDto>
{
    public override partial WorkflowDefinitionUpdateDto Map(WorkflowDefinitionDto source);
    public override partial void Map(WorkflowDefinitionDto source, WorkflowDefinitionUpdateDto destination);
}

[Mapper]
public partial class WorkflowDtoToWorkflowUpdateDtoBlazorMapper : MapperBase<WorkflowDto, WorkflowUpdateDto>
{
    public override partial WorkflowUpdateDto Map(WorkflowDto source);
    public override partial void Map(WorkflowDto source, WorkflowUpdateDto destination);
}

[Mapper]
public partial class WorkflowTemplateDtoToWorkflowTemplateUpdateDtoBlazorMapper : MapperBase<WorkflowTemplateDto, WorkflowTemplateUpdateDto>
{
    public override partial WorkflowTemplateUpdateDto Map(WorkflowTemplateDto source);
    public override partial void Map(WorkflowTemplateDto source, WorkflowTemplateUpdateDto destination);
}

[Mapper]
public partial class WorkflowStepTemplateDtoToWorkflowStepTemplateUpdateDtoBlazorMapper : MapperBase<WorkflowStepTemplateDto, WorkflowStepTemplateUpdateDto>
{
    public override partial WorkflowStepTemplateUpdateDto Map(WorkflowStepTemplateDto source);
    public override partial void Map(WorkflowStepTemplateDto source, WorkflowStepTemplateUpdateDto destination);
}

[Mapper]
public partial class DepartmentDtoToDepartmentUpdateDtoBlazorMapper : MapperBase<DepartmentDto, DepartmentUpdateDto>
{
    public override partial DepartmentUpdateDto Map(DepartmentDto source);
    public override partial void Map(DepartmentDto source, DepartmentUpdateDto destination);
}

[Mapper]
public partial class UnitDtoToUnitUpdateDtoBlazorMapper : MapperBase<UnitDto, UnitUpdateDto>
{
    public override partial UnitUpdateDto Map(UnitDto source);
    public override partial void Map(UnitDto source, UnitUpdateDto destination);
}

[Mapper]
public partial class WorkflowStepAssignmentDtoToWorkflowStepAssignmentUpdateDtoBlazorMapper : MapperBase<WorkflowStepAssignmentDto, WorkflowStepAssignmentUpdateDto>
{
    public override partial WorkflowStepAssignmentUpdateDto Map(WorkflowStepAssignmentDto source);
    public override partial void Map(WorkflowStepAssignmentDto source, WorkflowStepAssignmentUpdateDto destination);
}

[Mapper]
public partial class DocumentDtoToDocumentUpdateDtoBlazorMapper : MapperBase<DocumentDto, DocumentUpdateDto>
{
    public override partial DocumentUpdateDto Map(DocumentDto source);
    public override partial void Map(DocumentDto source, DocumentUpdateDto destination);
}

[Mapper]
public partial class DocumentFileDtoToDocumentFileUpdateDtoBlazorMapper : MapperBase<DocumentFileDto, DocumentFileUpdateDto>
{
    public override partial DocumentFileUpdateDto Map(DocumentFileDto source);
    public override partial void Map(DocumentFileDto source, DocumentFileUpdateDto destination);
}

[Mapper]
public partial class DocumentWorkflowInstanceDtoToDocumentWorkflowInstanceUpdateDtoBlazorMapper : MapperBase<DocumentWorkflowInstanceDto, DocumentWorkflowInstanceUpdateDto>
{
    public override partial DocumentWorkflowInstanceUpdateDto Map(DocumentWorkflowInstanceDto source);
    public override partial void Map(DocumentWorkflowInstanceDto source, DocumentWorkflowInstanceUpdateDto destination);
}

[Mapper]
public partial class DocumentAssignmentDtoToDocumentAssignmentUpdateDtoBlazorMapper : MapperBase<DocumentAssignmentDto, DocumentAssignmentUpdateDto>
{
    public override partial DocumentAssignmentUpdateDto Map(DocumentAssignmentDto source);
    public override partial void Map(DocumentAssignmentDto source, DocumentAssignmentUpdateDto destination);
}

[Mapper]
public partial class DocumentHistoryDtoToDocumentHistoryUpdateDtoBlazorMapper : MapperBase<DocumentHistoryDto, DocumentHistoryUpdateDto>
{
    public override partial DocumentHistoryUpdateDto Map(DocumentHistoryDto source);
    public override partial void Map(DocumentHistoryDto source, DocumentHistoryUpdateDto destination);
}

[Mapper]
public partial class ProjectDtoToProjectUpdateDtoBlazorMapper : MapperBase<ProjectDto, ProjectUpdateDto>
{
    public override partial ProjectUpdateDto Map(ProjectDto source);
    public override partial void Map(ProjectDto source, ProjectUpdateDto destination);
}

[Mapper]
public partial class ProjectMemberDtoToProjectMemberUpdateDtoBlazorMapper : MapperBase<ProjectMemberDto, ProjectMemberUpdateDto>
{
    public override partial ProjectMemberUpdateDto Map(ProjectMemberDto source);
    public override partial void Map(ProjectMemberDto source, ProjectMemberUpdateDto destination);
}

[Mapper]
public partial class ProjectTaskDtoToProjectTaskUpdateDtoBlazorMapper : MapperBase<ProjectTaskDto, ProjectTaskUpdateDto>
{
    public override partial ProjectTaskUpdateDto Map(ProjectTaskDto source);
    public override partial void Map(ProjectTaskDto source, ProjectTaskUpdateDto destination);
}

[Mapper]
public partial class ProjectTaskAssignmentDtoToProjectTaskAssignmentUpdateDtoBlazorMapper : MapperBase<ProjectTaskAssignmentDto, ProjectTaskAssignmentUpdateDto>
{
    public override partial ProjectTaskAssignmentUpdateDto Map(ProjectTaskAssignmentDto source);
    public override partial void Map(ProjectTaskAssignmentDto source, ProjectTaskAssignmentUpdateDto destination);
}

[Mapper]
public partial class ProjectTaskDocumentDtoToProjectTaskDocumentUpdateDtoBlazorMapper : MapperBase<ProjectTaskDocumentDto, ProjectTaskDocumentUpdateDto>
{
    public override partial ProjectTaskDocumentUpdateDto Map(ProjectTaskDocumentDto source);
    public override partial void Map(ProjectTaskDocumentDto source, ProjectTaskDocumentUpdateDto destination);
}

[Mapper]
public partial class NotificationDtoToNotificationUpdateDtoBlazorMapper : MapperBase<NotificationDto, NotificationUpdateDto>
{
    public override partial NotificationUpdateDto Map(NotificationDto source);
    public override partial void Map(NotificationDto source, NotificationUpdateDto destination);
}
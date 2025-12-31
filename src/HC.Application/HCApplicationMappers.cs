using HC.WorkflowStepAssignments;
using HC.Units;
using Volo.Abp.Identity;
using HC.Departments;
using HC.WorkflowStepTemplates;
using HC.WorkflowTemplates;
using HC.Workflows;
using HC.WorkflowDefinitions;
using HC.MasterDatas;
using System;
using HC.Shared;
using HC.Positions;
using System.Linq;
using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using HC.Books;

namespace HC;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class HCBookToBookDtoMapper : MapperBase<Book, BookDto>
{
    public override partial BookDto Map(Book source);
    public override partial void Map(Book source, BookDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class HCCreateUpdateBookDtoToBookMapper : MapperBase<CreateUpdateBookDto, Book>
{
    public override partial Book Map(CreateUpdateBookDto source);
    public override partial void Map(CreateUpdateBookDto source, Book destination);
}

[Mapper]
public partial class PositionToPositionDtoMappers : MapperBase<Position, PositionDto>
{
    public override partial PositionDto Map(Position source);
    public override partial void Map(Position source, PositionDto destination);
}

[Mapper]
public partial class PositionToPositionExcelDtoMappers : MapperBase<Position, PositionExcelDto>
{
    public override partial PositionExcelDto Map(Position source);
    public override partial void Map(Position source, PositionExcelDto destination);
}

[Mapper]
public partial class MasterDataToMasterDataDtoMappers : MapperBase<MasterData, MasterDataDto>
{
    public override partial MasterDataDto Map(MasterData source);
    public override partial void Map(MasterData source, MasterDataDto destination);
}

[Mapper]
public partial class MasterDataToMasterDataExcelDtoMappers : MapperBase<MasterData, MasterDataExcelDto>
{
    public override partial MasterDataExcelDto Map(MasterData source);
    public override partial void Map(MasterData source, MasterDataExcelDto destination);
}

[Mapper]
public partial class WorkflowDefinitionToWorkflowDefinitionDtoMappers : MapperBase<WorkflowDefinition, WorkflowDefinitionDto>
{
    public override partial WorkflowDefinitionDto Map(WorkflowDefinition source);
    public override partial void Map(WorkflowDefinition source, WorkflowDefinitionDto destination);
}

[Mapper]
public partial class WorkflowDefinitionToWorkflowDefinitionExcelDtoMappers : MapperBase<WorkflowDefinition, WorkflowDefinitionExcelDto>
{
    public override partial WorkflowDefinitionExcelDto Map(WorkflowDefinition source);
    public override partial void Map(WorkflowDefinition source, WorkflowDefinitionExcelDto destination);
}

[Mapper]
public partial class WorkflowToWorkflowDtoMappers : MapperBase<Workflow, WorkflowDto>
{
    public override partial WorkflowDto Map(Workflow source);
    public override partial void Map(Workflow source, WorkflowDto destination);
}

[Mapper]
public partial class WorkflowToWorkflowExcelDtoMappers : MapperBase<Workflow, WorkflowExcelDto>
{
    public override partial WorkflowExcelDto Map(Workflow source);
    public override partial void Map(Workflow source, WorkflowExcelDto destination);
}

[Mapper]
public partial class WorkflowTemplateToWorkflowTemplateDtoMappers : MapperBase<WorkflowTemplate, WorkflowTemplateDto>
{
    public override partial WorkflowTemplateDto Map(WorkflowTemplate source);
    public override partial void Map(WorkflowTemplate source, WorkflowTemplateDto destination);
}

[Mapper]
public partial class WorkflowTemplateToWorkflowTemplateExcelDtoMappers : MapperBase<WorkflowTemplate, WorkflowTemplateExcelDto>
{
    public override partial WorkflowTemplateExcelDto Map(WorkflowTemplate source);
    public override partial void Map(WorkflowTemplate source, WorkflowTemplateExcelDto destination);
}

[Mapper]
public partial class WorkflowTemplateWithNavigationPropertiesToWorkflowTemplateWithNavigationPropertiesDtoMapper : MapperBase<WorkflowTemplateWithNavigationProperties, WorkflowTemplateWithNavigationPropertiesDto>
{
    public override partial WorkflowTemplateWithNavigationPropertiesDto Map(WorkflowTemplateWithNavigationProperties source);
    public override partial void Map(WorkflowTemplateWithNavigationProperties source, WorkflowTemplateWithNavigationPropertiesDto destination);
}

[Mapper]
public partial class WorkflowToLookupDtoGuidMapper : MapperBase<Workflow, LookupDto<Guid>>
{
    public override partial LookupDto<Guid> Map(Workflow source);
    public override partial void Map(Workflow source, LookupDto<Guid> destination);

    public override void AfterMap(Workflow source, LookupDto<Guid> destination)
    {
        destination.DisplayName = source.Name;
    }
}

[Mapper]
public partial class WorkflowStepTemplateToWorkflowStepTemplateDtoMappers : MapperBase<WorkflowStepTemplate, WorkflowStepTemplateDto>
{
    public override partial WorkflowStepTemplateDto Map(WorkflowStepTemplate source);
    public override partial void Map(WorkflowStepTemplate source, WorkflowStepTemplateDto destination);
}

[Mapper]
public partial class WorkflowStepTemplateToWorkflowStepTemplateExcelDtoMappers : MapperBase<WorkflowStepTemplate, WorkflowStepTemplateExcelDto>
{
    public override partial WorkflowStepTemplateExcelDto Map(WorkflowStepTemplate source);
    public override partial void Map(WorkflowStepTemplate source, WorkflowStepTemplateExcelDto destination);
}

[Mapper]
public partial class WorkflowStepTemplateWithNavigationPropertiesToWorkflowStepTemplateWithNavigationPropertiesDtoMapper : MapperBase<WorkflowStepTemplateWithNavigationProperties, WorkflowStepTemplateWithNavigationPropertiesDto>
{
    public override partial WorkflowStepTemplateWithNavigationPropertiesDto Map(WorkflowStepTemplateWithNavigationProperties source);
    public override partial void Map(WorkflowStepTemplateWithNavigationProperties source, WorkflowStepTemplateWithNavigationPropertiesDto destination);
}

[Mapper]
public partial class DepartmentToDepartmentDtoMappers : MapperBase<Department, DepartmentDto>
{
    public override partial DepartmentDto Map(Department source);
    public override partial void Map(Department source, DepartmentDto destination);
}

[Mapper]
public partial class DepartmentToDepartmentExcelDtoMappers : MapperBase<Department, DepartmentExcelDto>
{
    public override partial DepartmentExcelDto Map(Department source);
    public override partial void Map(Department source, DepartmentExcelDto destination);
}

[Mapper]
public partial class DepartmentWithNavigationPropertiesToDepartmentWithNavigationPropertiesDtoMapper : MapperBase<DepartmentWithNavigationProperties, DepartmentWithNavigationPropertiesDto>
{
    public override partial DepartmentWithNavigationPropertiesDto Map(DepartmentWithNavigationProperties source);
    public override partial void Map(DepartmentWithNavigationProperties source, DepartmentWithNavigationPropertiesDto destination);
}

[Mapper]
public partial class IdentityUserToLookupDtoGuidMapper : MapperBase<IdentityUser, LookupDto<Guid>>
{
    public override partial LookupDto<Guid> Map(IdentityUser source);
    public override partial void Map(IdentityUser source, LookupDto<Guid> destination);

    public override void AfterMap(IdentityUser source, LookupDto<Guid> destination)
    {
        destination.DisplayName = source.Name;
    }
}

[Mapper]
public partial class UnitToUnitDtoMappers : MapperBase<Unit, UnitDto>
{
    public override partial UnitDto Map(Unit source);
    public override partial void Map(Unit source, UnitDto destination);
}

[Mapper]
public partial class UnitToUnitExcelDtoMappers : MapperBase<Unit, UnitExcelDto>
{
    public override partial UnitExcelDto Map(Unit source);
    public override partial void Map(Unit source, UnitExcelDto destination);
}

[Mapper]
public partial class WorkflowStepAssignmentToWorkflowStepAssignmentDtoMappers : MapperBase<WorkflowStepAssignment, WorkflowStepAssignmentDto>
{
    public override partial WorkflowStepAssignmentDto Map(WorkflowStepAssignment source);
    public override partial void Map(WorkflowStepAssignment source, WorkflowStepAssignmentDto destination);
}

[Mapper]
public partial class WorkflowStepAssignmentToWorkflowStepAssignmentExcelDtoMappers : MapperBase<WorkflowStepAssignment, WorkflowStepAssignmentExcelDto>
{
    public override partial WorkflowStepAssignmentExcelDto Map(WorkflowStepAssignment source);
    public override partial void Map(WorkflowStepAssignment source, WorkflowStepAssignmentExcelDto destination);
}

[Mapper]
public partial class WorkflowStepAssignmentWithNavigationPropertiesToWorkflowStepAssignmentWithNavigationPropertiesDtoMapper : MapperBase<WorkflowStepAssignmentWithNavigationProperties, WorkflowStepAssignmentWithNavigationPropertiesDto>
{
    public override partial WorkflowStepAssignmentWithNavigationPropertiesDto Map(WorkflowStepAssignmentWithNavigationProperties source);
    public override partial void Map(WorkflowStepAssignmentWithNavigationProperties source, WorkflowStepAssignmentWithNavigationPropertiesDto destination);
}

[Mapper]
public partial class WorkflowStepTemplateToLookupDtoGuidMapper : MapperBase<WorkflowStepTemplate, LookupDto<Guid>>
{
    public override partial LookupDto<Guid> Map(WorkflowStepTemplate source);
    public override partial void Map(WorkflowStepTemplate source, LookupDto<Guid> destination);

    public override void AfterMap(WorkflowStepTemplate source, LookupDto<Guid> destination)
    {
        destination.DisplayName = source.Name;
    }
}

[Mapper]
public partial class WorkflowTemplateToLookupDtoGuidMapper : MapperBase<WorkflowTemplate, LookupDto<Guid>>
{
    public override partial LookupDto<Guid> Map(WorkflowTemplate source);
    public override partial void Map(WorkflowTemplate source, LookupDto<Guid> destination);

    public override void AfterMap(WorkflowTemplate source, LookupDto<Guid> destination)
    {
        destination.DisplayName = source.Name;
    }
}
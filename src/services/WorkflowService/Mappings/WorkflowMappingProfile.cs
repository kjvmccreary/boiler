using AutoMapper;
using DTOs.Workflow;
using WorkflowService.Domain.Models;

namespace WorkflowService.Mappings;

public class WorkflowMappingProfile : Profile
{
    public WorkflowMappingProfile()
    {
        // WorkflowDefinition mappings
        CreateMap<WorkflowDefinition, WorkflowDefinitionDto>();
        CreateMap<CreateWorkflowDefinitionDto, WorkflowDefinition>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // WorkflowInstance mappings
        CreateMap<WorkflowInstance, WorkflowInstanceDto>()
            .ForMember(dest => dest.WorkflowDefinitionName, opt => opt.MapFrom(src => src.WorkflowDefinition.Name));

        // WorkflowTask mappings
        CreateMap<WorkflowTask, WorkflowTaskDto>();
        CreateMap<WorkflowTask, TaskSummaryDto>()
            .ForMember(dest => dest.WorkflowDefinitionName, opt => opt.MapFrom(src => src.WorkflowInstance.WorkflowDefinition.Name));

        // WorkflowEvent mappings
        CreateMap<WorkflowEvent, WorkflowEventDto>();
    }
}

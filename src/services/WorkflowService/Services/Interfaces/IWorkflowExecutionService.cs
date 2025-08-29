using System.Threading.Tasks;
using WorkflowService.Domain.Models;

namespace WorkflowService.Services.Interfaces
{
    public interface IWorkflowExecutionService
    {
        Task AdvanceAfterTaskCompletionAsync(WorkflowTask completedTask);
    }
}

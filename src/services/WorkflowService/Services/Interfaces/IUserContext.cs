namespace WorkflowService.Services.Interfaces;

public interface IUserContext
{
    int? UserId { get; }
    IReadOnlyCollection<string> Roles { get; }
}

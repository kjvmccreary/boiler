namespace WorkflowService.Engine.AutomaticActions.Executors;

/// <summary>
/// Trivial executor that does nothing successfully.
/// </summary>
public class NoopAutomaticActionExecutor : IAutomaticActionExecutor
{
    public string Kind => "noop";

    public Task<IAutomaticActionResult> ExecuteAsync(IAutomaticActionContext ctx)
        => Task.FromResult<IAutomaticActionResult>(AutomaticActionResult.Ok(new { message = "noop" }));
}

using System.Linq;
using System.Threading.Tasks;
using Xunit;

// Thin alias just to keep a test with the originally planned name.
// Leverages existing test logic indirectly (could be removed later).
namespace WorkflowService.Tests.Runtime;

public class WorkflowCompletionAliasTests : WorkflowCompletionTests
{
    [Fact]
    public async Task Instance_Completes_When_Last_Human_Task_Completed()
    {
        // Call the already proven path by invoking the existing test method logic.
        // For clarity & isolation you could duplicate logic instead; this keeps maintenance low.
        await Completing_Last_Human_Task_Should_Complete_Instance_Without_Cancelling_Task();
    }
}

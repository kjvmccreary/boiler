namespace WorkflowService.Domain.Dsl;

public static partial class NodeTypes
{
    public const string Join = "join";
}

public static partial class WorkflowNodeExtensions
{
    public static bool IsJoin(this WorkflowNode node) =>
        node.Type.Equals(NodeTypes.Join, StringComparison.OrdinalIgnoreCase);
}

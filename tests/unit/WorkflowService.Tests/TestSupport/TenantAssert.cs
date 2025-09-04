using DTOs.Common;
using FluentAssertions;

namespace WorkflowService.Tests.TestSupport;

public static class TenantAssert
{
    public static void ContainsOnlyTenant<T>(
        IEnumerable<T> items,
        int tenantId,
        Func<T,int> tenantSelector,
        string? because = null)
    {
        items.Should().OnlyContain(i => tenantSelector(i) == tenantId, because);
    }

    public static void NotFound(ApiResponseDto<object> resp) =>
        resp.Success.Should().BeFalse("expected not found")
            .And.Subject.As<ApiResponseDto<object>>().Message!.ToLower().Should().Contain("not found");
}

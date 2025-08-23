using UserService.PerformanceTests.Fixtures;
using Xunit;

namespace UserService.PerformanceTests.Collections;

/// <summary>
/// Collection definition to ensure all performance tests share a single fixture instance.
/// This is critical for performance tests to avoid the overhead of multiple application startups.
/// </summary>
[CollectionDefinition("Performance")]
public class PerformanceCollection : ICollectionFixture<PerformanceTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace WorkflowService.Tests.TestSupport;

public sealed class XunitOutputLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _output;
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

    public XunitOutputLoggerProvider(ITestOutputHelper output) => _output = output;

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, c => new XunitOutputLogger(_output, c));

    public void Dispose() { }

    private sealed class XunitOutputLogger : ILogger
    {
        private readonly ITestOutputHelper _output;
        private readonly string _category;
        public XunitOutputLogger(ITestOutputHelper output, string category) { _output = output; _category = category; }
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception? ex, Func<TState, Exception?, string> formatter)
        {
            try
            {
                _output.WriteLine($"[{level}] {_category} {formatter(state, ex)}");
                if (ex != null) _output.WriteLine(ex.ToString());
            }
            catch { /* ignore if output already disposed */ }
        }
        private sealed class NullScope : IDisposable { public static NullScope Instance { get; } = new(); public void Dispose() { } }
    }
}

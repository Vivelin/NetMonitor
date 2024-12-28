using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Vivelin.NetMonitor;

public class CustomConsoleFormatter(IOptionsMonitor<CustomConsoleFormatterOptions> options) : ConsoleFormatter("Custom")
{
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    private const string DefaultTimestampFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";

    protected CustomConsoleFormatterOptions Options => options.CurrentValue;

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var level = logEntry.LogLevel switch
        {
            LogLevel.Trace => "[TRACE]",
            LogLevel.Debug => "[DEBUG]",
            LogLevel.Information => "[INFO ]",
            LogLevel.Warning => "[WARN ]",
            LogLevel.Error => "[ERROR]",
            LogLevel.Critical => "[FATAL]",
            _ => null,
        };
        textWriter.Write(level);
        textWriter.Write(" ");

        var time = Options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
        var timestamp = time.ToString(Options.TimestampFormat ?? DefaultTimestampFormat, CultureInfo.InvariantCulture);
        textWriter.Write(timestamp);
        textWriter.Write(" ");

        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        textWriter.Write(message);
        textWriter.WriteLine();

    }
}

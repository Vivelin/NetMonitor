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
            LogLevel.Trace => "\e[90m[TRACE]",
            LogLevel.Debug => "\e[96m[DEBUG]",
            LogLevel.Information => "\e[92m [INFO]",
            LogLevel.Warning => "\e[93m [WARN]",
            LogLevel.Error => "\e[91m[ERROR]",
            LogLevel.Critical => "\e[91m[FATAL]",
            _ => null,
        };
        textWriter.Write(level);
        textWriter.Write("\e[2m ");

        var time = Options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
        var timestamp = time.ToString(Options.TimestampFormat ?? DefaultTimestampFormat, CultureInfo.InvariantCulture);
        textWriter.Write(timestamp);
        textWriter.Write(" ");

        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        textWriter.Write(logEntry.LogLevel switch
        {
            LogLevel.Critical => "\e[0;1;91m",
            _ => "\e[0m"
        });
        textWriter.Write(message);
        textWriter.WriteLine("\e[0m");
    }
}

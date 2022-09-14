using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Plataforma.Data.Interceptors;

public class WithNoLockInterceptor : DbCommandInterceptor {
    private static readonly Regex TableAliasRegex = new("(?<tableAlias>FROM [^)]*? AS \\[\\w+\\](?! WITH \\(NOLOCK\\)))", RegexOptions.IgnoreCase | RegexOptions.Multiline);

    // Used in DataSetExtensions to replace command with (nolock)
    public static bool WithNoLock { get; set; } = false;

    private static void FormatCommandText(ref DbCommand command) {
        if (!WithNoLock)
            return;
        command.CommandText = TableAliasRegex.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = new()) {
        FormatCommandText(ref command);
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = new()) {
        FormatCommandText(ref command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = new()) {
        FormatCommandText(ref command);
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result) {
        FormatCommandText(ref command);
        return base.ScalarExecuting(command, eventData, result);
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result) {
        FormatCommandText(ref command);
        return base.ReaderExecuting(command, eventData, result);
    }
    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result) {
        FormatCommandText(ref command);
        return base.NonQueryExecuting(command, eventData, result);
    }
}

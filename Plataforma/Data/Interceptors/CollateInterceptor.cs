using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Plataforma.Data.Interceptors;

public class CollateInterceptor : DbCommandInterceptor {
    private static readonly Regex LikeAliasRegex = new("(?<likeAlias>[^\\s]+) LIKE", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    private static readonly Regex CharIndexRegex = new("CHARINDEX\\((?<charindex1>[^\\,\\)]+)\\,(?<charindex2>[^\\,\\)]+)\\)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private const bool UseCollate = true;

    private static void FormatCommandText(ref DbCommand command) {
        if (!UseCollate)
#pragma warning disable CS0162
            return;
#pragma warning restore CS0162
        if (!command.CommandText.ToUpper().Contains("NULL LIKE"))
            command.CommandText = LikeAliasRegex.Replace(command.CommandText, "${likeAlias} COLLATE Latin1_General_CI_AI_WS LIKE");
        if (!command.CommandText.ToUpper().Contains("CHARINDEX(NULL") 
            && !command.CommandText.ToUpper().Contains(", NULL)"))
            command.CommandText = CharIndexRegex.Replace(command.CommandText, "CHARINDEX(${charindex1} COLLATE Latin1_General_CI_AI_WS, ${charindex2} COLLATE Latin1_General_CI_AI_WS)");
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

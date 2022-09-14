using Plataforma.Models.AutoLog;
using System.Collections.Generic;

namespace Plataforma.Models.Contracts;

/**
     * <remarks>
     *      The AutoLogItems property is excluded from AutoLoad. Use a filtered Include() and AsSplitQuery() if you need to use it.
     * </remarks>
     */
public interface IHasAutoLog {
    public AutoLog.AutoLogCreate AutoLogCreate { get; set; }
    public List<AutoLogUpdate> AutoLogUpdates { get; set; }
}
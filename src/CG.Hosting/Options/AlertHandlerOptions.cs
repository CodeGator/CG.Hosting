using CG.Options;
using System;

namespace CG.Hosting.Options
{
    /// <summary>
    /// This class represents configuration options for the hosted alert handler.
    /// </summary>
    public class AlertHandlerOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains options for audit alert processing.
        /// </summary>
        public AuditAlertOptions AuditAlerts { get; set; }

        /// <summary>
        /// This property contains options for debug alert processing.
        /// </summary>
        public DebugAlertOptions DebugAlerts { get; set; }

        /// <summary>
        /// This property contains options for information alert processing.
        /// </summary>
        public InformationAlertOptions InformationAlerts { get; set; }

        /// <summary>
        /// This property contains options for warning alert processing.
        /// </summary>
        public WarningAlertOptions WarningAlerts { get; set; }

        /// <summary>
        /// This property contains options for error alert processing.
        /// </summary>
        public ErrorAlertOptions ErrorAlerts { get; set; }

        /// <summary>
        /// This property contains options for critical error alert processing.
        /// </summary>
        public CriticalAlertOptions CriticalAlerts { get; set; }

        /// <summary>
        /// This property contains options for trace alert processing.
        /// </summary>
        public TraceAlertOptions TraceAlerts { get; set; }

        #endregion
    }
}

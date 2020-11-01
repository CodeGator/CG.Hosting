using CG.Options;
using System;

namespace CG.Hosting.Options
{
    /// <summary>
    /// This class represents configuration options for a standardized, hosted,
    /// ecosystem.
    /// </summary>
    public class StandardOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains alert handler options.
        /// </summary>
        public AlertHandlerOptions Alerts { get; set; }

        /// <summary>
        /// This property contains hosted service options.
        /// </summary>
        public ServiceOptions Services { get; set; }

        #endregion
    }
}

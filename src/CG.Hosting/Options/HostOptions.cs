using CG.Options;
using CG.Serilog.Options;
using System;

namespace CG.Hosting.Options
{
    /// <summary>
    /// This class represents configuration settings for hosting.
    /// </summary>
    public class HostOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains logging options.
        /// </summary>
        public SerilogOptions Logging { get; set; }

        #endregion
    }
}

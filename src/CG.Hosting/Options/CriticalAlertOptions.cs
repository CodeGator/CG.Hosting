using CG.Options;
using System;

namespace CG.Hosting.Options
{
    /// <summary>
    /// This class represents configuration options for critical alert processing.
    /// </summary>
    public class CriticalAlertOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains options for critical error emails.
        /// </summary>
        public EmailOptions Email { get; set; }

        #endregion
    }
}

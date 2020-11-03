using CG.Email.Options;
using CG.Options;
using CG.Serilog.Options;
using CG.Sms.Options;
using System;

namespace CG.Hosting.Options
{
    /// <summary>
    /// This class represents configuration options for hosted services.
    /// </summary>
    public class ServiceOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains logging service options.
        /// </summary>
        public SerilogServiceOptions Logging { get; set; }

        /// <summary>
        /// This property contains email service options.
        /// </summary>
        public EmailServiceOptions Email { get; set; }

        /// <summary>
        /// This property contains sms service options.
        /// </summary>
        public SmsServiceOptions Sms { get; set; }

        #endregion
    }
}

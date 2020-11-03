using CG.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Hosting.Options
{
    /// <summary>
    /// This class represents configuration options for sending sms messages.
    /// </summary>
    public class SmsOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property indicates whether sms messages are enabled, or not.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// This property contains one or more phone numbers, separated by
        /// commas, that represent the destination for sms messages.
        /// </summary>
        [Required]
        public string To { get; set; }

        /// <summary>
        /// This property contains a template for the sms message body.
        /// </summary>
        [Required]
        public string Body { get; set; }

        #endregion
    }
}

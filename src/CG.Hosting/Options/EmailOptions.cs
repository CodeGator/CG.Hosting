using CG.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Hosting.Options
{
    /// <summary>
    /// This class represents configuration options for sending emails.
    /// </summary>
    public class EmailOptions : OptionsBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property indicates whether emails are enabled, or not.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// This property contains one or more email addresses, separated by
        /// commas, that represent the destination for an email.
        /// </summary>
        [Required]
        public string To { get; set; }

        /// <summary>
        /// This property contains an email address that represents the source
        /// for an email.
        /// </summary>
        [Required]
        public string From { get; set; }

        /// <summary>
        /// This property contains the a template for the email subject.
        /// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// This property contains a template for the email body.
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// This property indicates the email contains HTML.
        /// </summary>
        public bool IsHtml { get; set; }

        #endregion
    }
}

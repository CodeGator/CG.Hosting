using System;

namespace CG.Hosting.Alerts
{
    /// <summary>
    /// This class utility contains well-known replacement token names.
    /// </summary>
    public static class TokenNames
    {
        // *******************************************************************
        // Fields.
        // *******************************************************************

        #region Fields

        /// <summary>
        /// This field represents the current application's friendly name.
        /// </summary>
        public const string APP = "%APP%";

        /// <summary>
        /// This field represents the current machine's name.
        /// </summary>
        public const string MN = "%MN%";

        /// <summary>
        /// This field represents the current user's name.
        /// </summary>
        public const string USER = "%USER%";

        /// <summary>
        /// This field represents the alert message.
        /// </summary>
        public const string MSG = "%MSG%";

        /// <summary>
        /// This field represents an exception associated with an alert.
        /// </summary>
        public const string EX = "%EX%";

        #endregion
    }
}

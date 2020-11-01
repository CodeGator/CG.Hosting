using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CG.Hosting.Alerts
{
    /// <summary>
    /// This class utility contains template rendering logic.
    /// </summary>
    public static class Template
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method renders the specified template using the specified 
        /// collection of named replacement tokens.
        /// </summary>
        /// <param name="template">The template to use for the operation.</param>
        /// <param name="tokens">The replacement tokens to use for the operation.</param>
        /// <returns>A string containing the rendered template.</returns>
        public static string Render(
            string template,
            IDictionary<string, string> tokens
            )
        {
            // Create a buffer for the template.
            var sb = new StringBuilder(template);

            // Look and replace tokens.
            foreach (var key in tokens.Keys)
            {
                var value = tokens[key];
                sb.Replace(key, value);
            }

            // Return the rendered template.
            return sb.ToString();
        }

        #endregion
    }
}

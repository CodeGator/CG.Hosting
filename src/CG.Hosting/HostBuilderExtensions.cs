using CG.Validations;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IHostBuilder"/>
    /// types.
    /// </summary>
    public static partial class HostBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds a standard hosting configuration to the specified
        /// host builder.
        /// </summary>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        public static IHostBuilder AddHosting(
            this IHostBuilder hostBuilder
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(hostBuilder, nameof(hostBuilder));

            // Add user secrets.
            hostBuilder.ConfigureAppConfiguration((hostingContext, builder) =>
            {
                // Build the configuration.
                var configuration = builder.Build();

                // Look for a user secrets id.
                var userSecretsId = configuration["UserSecrets:Id"];

                // Did we find one?
                if (string.IsNullOrEmpty(userSecretsId))
                {
                    // Configure the user secrets.
                    builder.AddUserSecrets(userSecretsId, true);
                }
            });

            // Return the builder.
            return hostBuilder;
        }

        #endregion
    }
}

using CG.Alerts;
using CG.Hosting.Alerts;
using CG.Hosting.Options;
using CG.Validations;
using Microsoft.Extensions.Hosting;
using System;

namespace CG.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IHost"/>
    /// type.
    /// </summary>
    public static partial class HostExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method sets a hosted handler for alerts processing. 
        /// </summary>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <param name="host">The host to use for the operation.</param>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// one or more of the arguments are missing, or NULL.</exception>
        /// <remarks>
        /// <para>
        /// The idea, with this method, is to replace the default alert handler
        /// with one that is capable of taking advantage of the services inside
        /// an <see cref="IHost"/> object, to process alerts with a wider variety
        /// of handling options. For more information see <seealso cref="HostedAlertHandler{TOptions}"/>
        /// </para>
        /// </remarks>
        public static IHost SetHostedAlertHandler<TOptions>(
            this IHost host
            ) where TOptions : StandardOptions, new()
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(host, nameof(host));

            // Set the default handler.
            Alert.Instance().SetHandler(
                new HostedAlertHandler<TOptions>(host)
                );

            // Return the host.
            return host;
        }

        #endregion
    }
}

using CG.Validations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
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
        /// This method runs the specified delegate asynchronously.
        /// </summary>
        /// <param name="host">The host to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>A task to perform the operation.</returns>
        public static async Task RunDelegateAsync(
            this IHost host,
            Action<IHost, CancellationToken> action,
            CancellationToken token = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(host, nameof(host))
                .ThrowIfNull(action, nameof(action));

            // Run the action asynchronously.
            await Task.Run(
                () => action(host, token),
                token
                ).ConfigureAwait(false);
        }

        // *******************************************************************

        /// <summary>
        /// This method runs the specified delegate.
        /// </summary>
        /// <param name="host">The host to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        public static void RunDelegate(
            this IHost host,
            Action<IHost> action
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(host, nameof(host))
                .ThrowIfNull(action, nameof(action));

            // Run the action.
            action(host);
        }

        #endregion
    }
}

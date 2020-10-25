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
        /// This method runs a delegate within the context of the specified <see cref="IHost"/> 
        /// object.
        /// </summary>
        /// <param name="host">The host to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>A task to perform the operation.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demostrates a typical use of the <see cref="RunDelegateAsync(IHost, Action{IHost, CancellationToken}, CancellationToken)"/>
        /// method:
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     Host.CreateDefaultBuilder()
        ///         .Build()
        ///         .RunDelegateAsync((host, token) => 
        ///         {
        ///             Console.WriteLine("Hello World");
        ///         });
        /// }
        /// </code>
        /// </example>
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
        /// This method runs a delegate within the context of the specified <see cref="IHost"/> 
        /// object.
        /// </summary>
        /// <param name="host">The host to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demostrates a typical use of the <see cref="RunDelegate(IHost, Action{IHost})"/>
        /// method:
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     Host.CreateDefaultBuilder()
        ///         .Build()
        ///         .RunDelegate((host) => 
        ///         {
        ///             Console.WriteLine("Hello World");
        ///         });
        /// }
        /// </code>
        /// </example>
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


namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IHostBuilder"/>
    /// types, for registering types related to host building.
    /// </summary>
    public static partial class HostingHostBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method runs a delegate within the context of the specified <see cref="IHostBuilder"/> 
        /// object.
        /// </summary>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <param name="hostDelegate">The delegate to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demonstrates a typical use of the <see cref="RunDelegate(IHostBuilder, Action{IHost, CancellationToken}, CancellationToken)"/>
        /// method:
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     Host.CreateDefaultBuilder()
        ///         .RunDelegate((host, token) => 
        ///         {
        ///             Console.WriteLine("Hello World");
        ///         });
        /// }
        /// </code>
        /// </example>
        public static void RunDelegate(
           this IHostBuilder hostBuilder,
           Action<IHost, CancellationToken> hostDelegate,
           CancellationToken cancellationToken = default
           )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(hostBuilder, nameof(hostBuilder))
                .ThrowIfNull(hostDelegate, nameof(hostDelegate));

            // Create the host.
            var host = hostBuilder.UseConsoleLifetime()
                .Build();

            try
            {
                // Invoke the delegate.
                hostDelegate(
                    host,
                    cancellationToken
                    );
            }
            finally
            {
                // Stop the host.
                host.StopAsync(
                    cancellationToken
                    ).Wait();
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method runs a delegate within the context of the specified <see cref="IHostBuilder"/> 
        /// object.
        /// </summary>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        public static async Task RunDelegateAsync(
            this IHostBuilder hostBuilder,
            Action<IHost, CancellationToken> action,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(hostBuilder, nameof(hostBuilder))
                .ThrowIfNull(action, nameof(action));

            // Create the host.
            var host = hostBuilder.UseConsoleLifetime()
                .Build();

            try
            {
                // Run the action asynchronously.
                await Task.Run(
                    () => action(host, cancellationToken),
                    cancellationToken
                    ).ConfigureAwait(false);
            }
            finally
            {
                // Stop the host.
                await host.StopAsync(
                    cancellationToken
                    ).ConfigureAwait(false);
            }
        }

        #endregion
    }
}

﻿
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
        ///         }).Result;
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

            try
            {
                // Run the action asynchronously.
                await Task.Run(
                    () => action(host, token),
                    token
                    ).ConfigureAwait(false);
            }
            finally
            {
                // Stop the host.
                await host.StopAsync(
                    token
                    ).ConfigureAwait(false);
            }
        }

        // *******************************************************************

        /// <summary>
        /// This method runs a delegate within the context of the specified <see cref="IHost"/> 
        /// object.
        /// </summary>
        /// <param name="host">The host to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demostrates a typical use of the <see cref="RunDelegate(IHost, Action{IHost, CancellationToken}, CancellationToken)"/>
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
            Action<IHost, CancellationToken> action,
            CancellationToken cancellationToken = default
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(host, nameof(host))
                .ThrowIfNull(action, nameof(action));

            try
            {
                // Run the action.
                action(
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
        /// This method runs a delegate within the context of the specified <see cref="IHost"/> 
        /// object, allowing a single instance at any given time to run on the 
        /// given machine.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated .NET program</typeparam>
        /// <param name="host">The host to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        /// <returns>True if the delegate was run; false otherwise.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demonstrates how to use the <see cref="RunOnce{TProgram}(IHost, Action{IHost})"/> 
        /// method:
        /// <code>
        /// class TestProgram
        /// {
        ///     static void Main(string[] args)
        ///     {
        ///        Host.CreateDefaultBuilder()
        ///            .Build()
        ///           .RunOnce(host => 
        ///           {
        ///               Console.WriteLine("Hello World");
        ///           });
        ///     }
        /// }
        /// </code>
        /// </example>
        public static bool RunOnce<TProgram>(
            this IHost host,
            Action<IHost> action
            ) where TProgram : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(host, nameof(host))
                .ThrowIfNull(action, nameof(action));

            // Get a friendly name for the application.
            var appName = AppDomain.CurrentDomain.FriendlyNameEx(true);

            // Create a safe mutex name.
            var mutexName = $"Global\\{{{appName.Replace('\\', '_').Replace(':', '_')}}}";

            // Create a mutex to control access to the delegate.
            using (var mutex = new Mutex(false, mutexName))
            {
                try
                {
                    // Are we the first?
                    if (mutex.WaitOne(TimeSpan.FromSeconds(1), false))
                    {
                        try
                        {
                            // Run the delegate.
                            action.Invoke(host);

                            // Return the results.
                            return true;
                        }
                        finally
                        {
                            // Cleanup the mutex.
                            mutex.ReleaseMutex();
                        }
                    }
                }
                catch (AbandonedMutexException ex)
                {
                    // If we get here then, likely, whatever application/thread owned the mutex at the time
                    //   we tried to wait for it crashed. So now the O/S considers the mutex "abandoned". 

                    // Do we have a reference to the abandoned mutex?
                    if (null != ex.Mutex)
                    {
                        // Release the mutex.
                        ex.Mutex.ReleaseMutex();
                    }

                    // Run the delegate.
                    action.Invoke(host);
                }
                finally
                {
                    // Stop the host.
                    host.StopAsync().Wait();
                }
            }

            // Return the results.
            return false;
        }

        // *******************************************************************

        /// <summary>
        /// This method runs a delegate within the context of the specified <see cref="IHost"/> 
        /// object, allowing a single instance at any given time to run on the 
        /// given machine.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated .NET program</typeparam>
        /// <param name="host">The host to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if the delegate was run; false otherwise.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <remarks>
        /// <para>
        /// This method is intended to wrap the logic in the main entry point of
        /// a .NET process. It then prevents that logic from being run more than 
        /// once simultaneously. 
        /// </para>
        /// </remarks>
        /// <example>
        /// This example demonstrates how to use the <see cref="RunOnceAsync{TProgram}(IHost, Action{IHost}, CancellationToken)"/> 
        /// method:
        /// <code>
        /// class TestProgram
        /// {
        ///     static async Task Main(string[] args)
        ///     {
        ///        await Host.CreateDefaultBuilder()
        ///            .Build()
        ///           .RunOnceAsync((host) => 
        ///           {
        ///               Console.WriteLine("Hello World");
        ///           });
        ///     }
        /// }
        /// </code>
        /// </example>
        public static async Task<bool> RunOnceAsync<TProgram>(
            this IHost host,
            Action<IHost> action,
            CancellationToken cancellationToken = default
            ) where TProgram : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(host, nameof(host))
                .ThrowIfNull(action, nameof(action));

            // Get a friendly name for the application.
            var appName = AppDomain.CurrentDomain.FriendlyNameEx(true);

            // Create a safe mutex name.
            var mutexName = $"Global\\{{{appName.Replace('\\', '_').Replace(':', '_')}}}";

            // Create a mutex to control access to the delegate.
            using (var mutex = new Mutex(false, mutexName))
            {
                try
                {
                    // Are we the first?
                    if (mutex.WaitOne(TimeSpan.FromSeconds(1), false))
                    {
                        try
                        {
                            // Run the delegate.
                            await Task.Run(
                                () => action(host),
                                cancellationToken
                                ).ConfigureAwait(false);

                            // Return the results.
                            return true;
                        }
                        finally
                        {
                            // Cleanup the mutex.
                            mutex.ReleaseMutex();
                        }
                    }
                }
                catch (AbandonedMutexException ex)
                {
                    // If we get here then, likely, whatever application/thread owned the mutex at the time
                    //   we tried to wait for it, crashed. So now the O/S considers the mutex "abandoned". 

                    // Do we have a reference to the abandoned mutex?
                    if (null != ex.Mutex)
                    {
                        // Release the mutex.
                        ex.Mutex.ReleaseMutex();
                    }

                    // Run the delegate.
                    await Task.Run(
                        () => action(host),
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

            // Return the results.
            return false;
        }

        #endregion
    }
}

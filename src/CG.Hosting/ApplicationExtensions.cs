using CG.Reflection;
using CG.Utilities;
using CG.Validations;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IApplication"/>
    /// type.
    /// </summary>
    public static partial class ApplicationExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method executes the specified delegate in a synchronized context, 
        /// allowing only one instance at a time to run on the given machine.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated .NET program</typeparam>
        /// <param name="application">The application to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        /// <returns>True if the delegate was run; false otherwise.</returns>
        [DebuggerStepThrough]
        public static bool RunOnce<TProgram>(
            this IApplication application,
            Action action
            ) where TProgram : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(application, nameof(application))
                .ThrowIfNull(action, nameof(action));
            
            // Should we create a default, minimal logger?
            if (null == Log.Logger || "SilentLogger" == Log.Logger.GetType().Name)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger();
            }

            try
            {
                // Get a friendly name for the application.
                var appName = AppDomain.CurrentDomain.FriendlyNameEx(true);

                // Get the file version for the program's assembly.
                var appVersion = typeof(TProgram).Assembly.ReadFileVersion();

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
                                // Tell the world what we're doing.
                                Log.Information($"starting '{appName}' - '{appVersion}'");

                                // Make a pretty console title.
                                Console.Title = $"{appName} - {appVersion}";

                                // Run the delegate.
                                action.Invoke();

                                // Return the results.
                                return true;
                            }
                            finally
                            {
                                // Tell the world what we're doing.
                                Log.Information($"stopping '{appName}' - '{appVersion}'");

                                // Cleanup the mutex.
                                mutex.ReleaseMutex();
                            }
                        }
                    }
                    catch (AbandonedMutexException ex)
                    {
                        // Tell the world what happened.
                        Log.Warning($"failed to wait for mutex: '{mutexName}', error: {ex.Message}");

                        // If we get here then, likely, whatever application/thread owned the mutex at the time
                        //   we tried to wait for it crashed. So now the O/S considers the mutex "abandoned". 

                        // Do we have a reference to the abandoned mutex?
                        if (null != ex.Mutex)
                        {
                            // Release the mutex.
                            ex.Mutex.ReleaseMutex();
                        }

                        // Tell the world what happened.
                        Log.Information($"retrying execution of '{appName}' - '{appVersion}'");

                        // Retry the operation.
                        if (false == Retry.Instance().Execute(
                            () => application.RunOnce<TProgram>(action)
                            ))
                        {
                            // Tell the world what happened.
                            Log.Error($"unable to run '{appName}' - '{appVersion}', error: {ex.Message}");

                            // Retry failed, so panic!
                            throw;
                        }
                    }
                }

                // Tell the world what happened.
                Log.Information($"'{appName}' - '{appVersion}' was already running.");
            }
            catch (Exception ex)
            {
                // Should we create a default, minimal logger?
                if (null == Log.Logger || "SilentLogger" == Log.Logger.GetType().Name)
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .CreateLogger();
                }

                // Tell the world what happened.
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                // Flush the log.
                Log.CloseAndFlush();
            }

            // Return the results.
            return false;
        }

        // *******************************************************************

        /// <summary>
        /// This method executes the specified delegate in a synchronized context, 
        /// allowing only one instance at a time to run on the given machine.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated .NET program</typeparam>
        /// <param name="application">The application to use for the operation.</param>
        /// <param name="action">The delegate to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if the delegate was run; false otherwise.</returns>
        [DebuggerStepThrough]
        public static async Task<bool> RunOnceAsync<TProgram>(
            this IApplication application,
            Action action,
            CancellationToken cancellationToken = default
            ) where TProgram : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(action, nameof(action));

            // Should we create a default, minimal logger?
            if (null == Log.Logger || "SilentLogger" == Log.Logger.GetType().Name)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger();
            }

            try
            {
                // Get a friendly name for the application.
                var appName = AppDomain.CurrentDomain.FriendlyNameEx(true);

                // Get the file version for the program's assembly.
                var appVersion = typeof(TProgram).Assembly.ReadFileVersion();

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
                                // Tell the world what we're doing.
                                Log.Information($"starting '{appName}' - '{appVersion}'");

                                // Make a pretty console title.
                                Console.Title = $"{appName} - {appVersion}";

                                // Run the delegate.
                                await Task.Run(
                                    action,
                                    cancellationToken
                                    ).ConfigureAwait(false);

                                // Return the results.
                                return true;
                            }
                            finally
                            {
                                // Tell the world what we're doing.
                                Log.Information($"stopping '{appName}' - '{appVersion}'");

                                // Cleanup the mutex.
                                mutex.ReleaseMutex();
                            }
                        }
                    }
                    catch (AbandonedMutexException ex)
                    {
                        // Tell the world what happened.
                        Log.Warning($"failed to wait for mutex: '{mutexName}', error: {ex.Message}");

                        // If we get here then, likely, whatever application/thread owned the mutex at the time
                        //   we tried to wait for it crashed. So now the O/S considers the mutex "abandoned". 

                        // Do we have a reference to the abandoned mutex?
                        if (null != ex.Mutex)
                        {
                            // Release the mutex.
                            ex.Mutex.ReleaseMutex();
                        }

                        // Tell the world what happened.
                        Log.Information($"retrying execution of '{appName}' - '{appVersion}'");

                        // Retry the operation.
                        if (false == await Retry.Instance().ExecuteAsync(
                            () => application.RunOnceAsync<TProgram>(action)
                            ))
                        {
                            // Tell the world what happened.
                            Log.Error($"unable to run '{appName}' - '{appVersion}', error: {ex.Message}");

                            // Retry failed, so panic!
                            throw;
                        }
                    }
                }

                // Tell the world what happened.
                Log.Information($"'{appName}' - '{appVersion}' was already running.");
            }
            catch (Exception ex)
            {
                // Should we create a default, minimal logger?
                if (null == Log.Logger || "SilentLogger" == Log.Logger.GetType().Name)
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .CreateLogger();
                }

                // Tell the world what happened.
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                // Flush the log.
                Log.CloseAndFlush();
            }

            // Return the results.
            return false;

        }

        #endregion
    }
}

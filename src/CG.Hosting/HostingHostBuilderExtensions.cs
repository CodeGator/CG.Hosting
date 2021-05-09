using CG.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        /// This example demostrates a typical use of the <see cref="RunDelegate(IHostBuilder, Action{IHost, CancellationToken}, CancellationToken)"/>
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

        // *******************************************************************

        /// <summary>
        /// This method adds standard hosting configuration and services to the 
        /// specified host builder, including user secrets and serilog based logging.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated hosting program.</typeparam>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demonstrates a typical use of the <see cref="AddStandardExtensions{TProgram}(IHostBuilder)"/>
        /// method:
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     Host.CreateDefaultBuilder()
        ///         .AddStandardExtensions{Program}() 
        ///         .ConfigureWebHost(hostBuilder =>
        ///         {
        ///             hostBuilder.UseStandardExtensions(); // This call also required.
        ///         })
        ///         .Build()
        ///         .Run();
        /// }
        /// </code>
        /// </example>
        public static IHostBuilder AddStandardExtensions<TProgram>(
            this IHostBuilder hostBuilder
            ) where TProgram : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(hostBuilder, nameof(hostBuilder));

            // Add user secrets.
            hostBuilder.ConfigureAppConfiguration((hostingContext, builder) =>
            {
                builder.AddUserSecrets<TProgram>(true, true);
            });

            // Add Serilog services.
            hostBuilder.AddStandardSerilog();

            // Return the builder.
            return hostBuilder;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds standard hosting configuration and services to the 
        /// specified host builder, including user secrets, program options, and 
        /// serilog based logging.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated hosting program.</typeparam>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demonstrates a typical use of the <see cref="AddStandardExtensions{TProgram, TOptions}(IHostBuilder)"/>
        /// method:
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     Host.CreateDefaultBuilder()
        ///         .AddStandardExtensions{Program, MyOptions}() 
        ///         .ConfigureWebHost(hostBuilder =>
        ///         {
        ///             hostBuilder.UseStandardExtensions(); // This call also required.
        ///         })
        ///         .Build()
        ///         .Run();
        /// }
        /// </code>
        /// </example>
        public static IHostBuilder AddStandardExtensions<TProgram, TOptions>(
            this IHostBuilder hostBuilder
            ) where TProgram : class
              where TOptions : class, new()
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(hostBuilder, nameof(hostBuilder));

            // Add user secrets.
            hostBuilder.ConfigureAppConfiguration((hostingContext, builder) =>
            {
                builder.AddUserSecrets<TProgram>(true, true);
            });

            // Add hosting options.
            hostBuilder.ConfigureServices((context, services) =>
            {
                // Create a service provider.
                var serviceProvider = services.BuildServiceProvider();

                // Get the configuration.
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                // Configure the host options.
                services.ConfigureOptions<TOptions>(configuration);
            });

            // Add Serilog services.
            hostBuilder.AddStandardSerilog();

            // Add email services.
            hostBuilder.ConfigureServices((context, services) =>
            {
                // Create a service provider.
                var serviceProvider = services.BuildServiceProvider();

                // Get the configuration.
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                // Configure the email service.
                services.AddEmail(
                    configuration.GetSection("Services:Email")
                    );

                // Configure the sms service.
                services.AddSms(
                    configuration.GetSection("Services:Sms")
                    );
            });

            // Return the builder.
            return hostBuilder;
        }

        #endregion
    }
}

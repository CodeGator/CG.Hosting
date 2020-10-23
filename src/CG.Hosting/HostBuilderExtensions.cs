using CG;
using CG.DataAnnotations;
using CG.DataAnnotations.Options;
using CG.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
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
        /// This method adds standard hosting configuration to the specified
        /// host builder, including user secrets and serilog logging.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated hosting program.</typeparam>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
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
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(
                    hostingContext.Configuration
                    )
                    .Enrich.WithExceptionDetails()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", AppDomain.CurrentDomain.FriendlyNameEx(true))
                    .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
#if DEBUG
                loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
            });

            // Return the builder.
            return hostBuilder;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds standard hosting configuration to the specified
        /// host builder, including user secrets, program options, and serilog 
        /// logging.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated hosting program.</typeparam>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
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

            // Add standard options.
            hostBuilder.ConfigureServices((context, services) =>
            {
                // Create the service provider.
                var serviceProvider = services.BuildServiceProvider();

                // Get the configuration.
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                // Create default options.
                var options = new TOptions();

                // Bind the options to the configuration.
                configuration.Bind(options);

                // Validate the options, if possible.
                (options as OptionsBase)?.ThrowIfInvalid();

                // Add the populated options as a service.
                services.AddSingleton<IOptions<TOptions>>(
                    new OptionsWrapper<TOptions>(options)
                    );
            });

            // Add Serilog services.
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(
                    hostingContext.Configuration
                    )
                    .Enrich.WithExceptionDetails()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", AppDomain.CurrentDomain.FriendlyNameEx(true))
                    .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
#if DEBUG
                loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
            });

            // Return the builder.
            return hostBuilder;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds standard hosting configuration to the specified
        /// host builder, including user secrets, program options, and serilog 
        /// logging.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated hosting program.</typeparam>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <param name="sectionName">The configuration section to use for the operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        public static IHostBuilder AddStandardExtensions<TProgram, TOptions>(
            this IHostBuilder hostBuilder,
            string sectionName
            ) where TProgram : class
              where TOptions : class, new()
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(hostBuilder, nameof(hostBuilder))
                .ThrowIfNullOrEmpty(sectionName, nameof(sectionName));

            // Add user secrets.
            hostBuilder.ConfigureAppConfiguration((hostingContext, builder) =>
            {
                builder.AddUserSecrets<TProgram>(true, true);
            });

            // Add standard options.
            hostBuilder.ConfigureServices((context, services) =>
            {
                // Create the service provider.
                var serviceProvider = services.BuildServiceProvider();

                // Get the configuration.
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                // Get the section.
                var section = configuration.GetSection(sectionName);

                // Create default options.
                var options = new TOptions();

                // Bind the options to the section.
                section.Bind(options);

                // Validate the options, if possible.
                (options as OptionsBase)?.ThrowIfInvalid();

                // Add the populated options as a service.
                services.AddSingleton<IOptions<TOptions>>(
                    new OptionsWrapper<TOptions>(options)
                    );
            });

            // Add Serilog services.
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(
                    hostingContext.Configuration.GetSection(sectionName)
                    )
                    .Enrich.WithExceptionDetails()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", AppDomain.CurrentDomain.FriendlyNameEx(true))
                    .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
#if DEBUG
                loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
            });

            // Return the builder.
            return hostBuilder;
        }

        #endregion
    }
}

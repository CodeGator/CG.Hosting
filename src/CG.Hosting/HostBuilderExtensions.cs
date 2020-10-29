using CG.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            hostBuilder.AddSerilog();

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
            hostBuilder.AddSerilog();

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
        /// <param name="sectionName">The configuration section to use for the operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demonstrates a typical use of the <see cref="AddStandardExtensions{TProgram, TOptions}(IHostBuilder, string)"/>
        /// method:
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     Host.CreateDefaultBuilder()
        ///         .AddStandardExtensions{Program, MyOptions}("MySection") 
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

                // Configure the options.
                services.ConfigureOptions<TOptions>(section);
            });

            // Add Serilog services.
            hostBuilder.AddSerilog(sectionName);

            // Return the builder.
            return hostBuilder;
        }

        #endregion
    }
}

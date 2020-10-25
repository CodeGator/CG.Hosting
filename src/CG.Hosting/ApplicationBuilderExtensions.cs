using CG.Validations;
using Microsoft.AspNetCore.Builder;
using Serilog;
using System;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IApplicationBuilder"/>
    /// type.
    /// </summary>
    public static partial class ApplicationBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds middleware to support a standard hosting configuration, 
        /// including user secrets, program options, and serilog logging.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use 
        /// for the operation.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/>
        /// parameter for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demostrates a typical use of the <see cref="UseStandardExtensions(IApplicationBuilder)"/>
        /// method:
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     Host.CreateDefaultBuilder()
        ///         .AddStandardExtensions{Program, MyOptions}() // This call also required.
        ///         .ConfigureWebHost(hostBuilder =>
        ///         {
        ///             hostBuilder.UseStandardExtensions();
        ///         })
        ///         .Build()
        ///         .Run();
        /// }
        /// </code>
        /// </example>
        public static IApplicationBuilder UseStandardExtensions(
            this IApplicationBuilder applicationBuilder
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder));

            // Use serilog.
            applicationBuilder.UseSerilogRequestLogging();

            // Return the builder.
            return applicationBuilder;
        }

        #endregion
    }
}

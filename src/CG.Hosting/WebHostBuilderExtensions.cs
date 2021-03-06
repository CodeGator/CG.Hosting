﻿using CG.Validations;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IWebHostBuilder"/>
    /// types.
    /// </summary>
    public static partial class WebHostBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds middleware to support a standard hosting configuration, 
        /// including user secrets, program options, and serilog logging.
        /// </summary>
        /// <param name="webHostBuilder">The application builder to use 
        /// for the operation.</param>
        /// <returns>The value of the <paramref name="webHostBuilder"/>
        /// parameter for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// any of the arguments are missing, or NULL.</exception>
        /// <example>
        /// This example demostrates a typical use of the <see cref="UseStandardExtensions(IWebHostBuilder)"/>
        /// method:
        /// <code>
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     Host.CreateDefaultBuilder()
        ///         .AddStandardExtensions{Program}() // This call also required.
        ///         .ConfigureWebHost(hostBuilder =>
        ///         {
        ///             hostBuilder.UseStandardExtensions();
        ///         })
        ///         .Build()
        ///         .Run();
        /// }
        /// </code>
        /// </example>
        public static IWebHostBuilder UseStandardExtensions(
            this IWebHostBuilder webHostBuilder
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(webHostBuilder, nameof(webHostBuilder));

            // Use serilog.
            webHostBuilder.UseStandardSerilog();

            // Return the builder.
            return webHostBuilder;
        }

        #endregion
    }
}

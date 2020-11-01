using CG.Hosting.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CG.Hosting
{
    /// <summary>
    /// This class utility provides convenience methods for creating instances of 
    /// <see cref="IHostBuilder"/> with pre-configured defaults.
    /// </summary>
    public static class StandardHost
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method creates an <see cref="IHostBuilder"/> object preconfigured
        /// with a standard set of extensions.
        /// </summary>
        /// <typeparam name="TProgram">The type of associated program.</typeparam>
        /// <typeparam name="TOptions">The type of associated options.</typeparam>
        /// <returns>An <see cref="IHostBuilder"/> object.</returns>
        public static IHostBuilder CreateStandardBuilder<TProgram, TOptions>()
            where TProgram : class
            where TOptions : StandardOptions, new()
        {
            // Create the host builder.
            var hostBuilder = Host.CreateDefaultBuilder()
                .AddStandardExtensions<TProgram, TOptions>();

            // Return the host builder.
            return hostBuilder;
        }

        #endregion
    }
}

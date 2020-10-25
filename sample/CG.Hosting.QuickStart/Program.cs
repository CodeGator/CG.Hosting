using CG.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CG.Hosting.QuickStart
{
    /// <summary>
    /// This class represents options for this sample.
    /// </summary>
    class MyOptions : OptionsBase
    {
        public string A { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder()
                .AddStandardExtensions<Program, MyOptions>()
                .ConfigureWebHost(hostBuilder =>
                {
                    hostBuilder.UseStandardExtensions();
                })
                .Build()
                .RunDelegate(h =>
            {
                // Let's verify that we have a serilog logger and our options.
                
                var options = h.Services.GetRequiredService<IOptions<MyOptions>>().Value;
                var logger = h.Services.GetRequiredService<ILogger<Program>>();

                // TODO : use the debugger to verify that the options have our property
                //   and the logger is, in fact, a serilog logger.

                Console.WriteLine();
                Console.WriteLine("press any key to exit.");
                Console.ReadKey();
            });

        }
    }
}

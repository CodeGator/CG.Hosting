using Microsoft.Extensions.Hosting;
using System;

namespace CG.Hosting.QuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .RunDelegate((host, token) =>
                {
                    Console.WriteLine("Delegate called - press any key to exit.");
                    Console.ReadKey();
                });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args);
    }
}
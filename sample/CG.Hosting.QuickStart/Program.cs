using CG.Alerts;
using CG.Hosting.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace CG.Hosting.QuickStart
{
    class MyOptions : StandardOptions
    {
        public string A { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StandardHost.CreateStandardBuilder<Program, MyOptions>()
                .Build()
                .SetHostedAlertHandler<MyOptions>()
                .RunDelegate(h =>
            {
                Console.WriteLine("about to raise a critical error alert ...");
                Alert.Instance().RaiseCritical("this is a test error");

                Console.WriteLine();
                Console.WriteLine("press any key to exit.");
                Console.ReadKey();
            });
        }
    }
}

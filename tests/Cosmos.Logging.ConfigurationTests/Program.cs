﻿using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Cosmos.Logging.ConfigurationTests {
    class Program {
        public Program() {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            Configuration = builder.Build();

            // ReSharper disable once UnusedVariable
            var config = Configuration.GetSection("Logging").Get<LoggingConfiguration>();

            LOGGER.Initialize().RunsOnConsole()
                .AddSampleLog()
                .AllDone();

        }

        private IConfigurationRoot Configuration { get; set; }

        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args) {
            try {

                var program = new Program();
                program.Process();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }

            Console.ReadLine();
        }

        public void Process() {
            var logger = LOGGER.GetLogger<Program>();
            logger.LogInformation("hello");
        }
    }
}
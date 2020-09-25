using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MSFSFlightFollowing
{
   public class Program
   {
      public static void Main(string[] args)
      {
         IHost host = CreateHostBuilder(args).Build();
         ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

         try
         {

#if RELEASE
         Process.Start(new ProcessStartInfo("http://localhost:9000") { UseShellExecute = true });
#endif
            new Task(() => MessageWindow.MessageLoop()).Start();

            host.RunAsync();
            logger.LogInformation("The application is running.\n\n" +
               "If your browser didn't automatically open a window, go to {0}.\n" +
               "Other devices on your network can go to {1}.\n\n" +
               "To shutdown the app, just close this window.", "http://localhost:9000", $"http://{Environment.MachineName.ToLower()}:9000");
            host.WaitForShutdown();
         }
         catch (Exception ex)
         {
            logger.LogError("There was an error and the app is unable to start: {0}", ex.Message);
         }
      }

      public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
#if RELEASE
                 webBuilder.UseContentRoot(AppContext.BaseDirectory);
#endif
                 webBuilder.UseStartup<Startup>();
              })
              .UseSerilog((hostingContext, loggerConfig) => loggerConfig.ReadFrom.Configuration(hostingContext.Configuration));
   }
}

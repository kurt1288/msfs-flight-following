using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSFSFlightFollowing.Models;

namespace MSFSFlightFollowing
{
   public class Startup
   {
      private IWebHostEnvironment Env { get; set; }

      public Startup(IWebHostEnvironment env)
      {
         Env = env;
      }

      // This method gets called by the runtime. Use this method to add services to the container.
      // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
      public void ConfigureServices(IServiceCollection services)
      {
         services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
         IMvcBuilder builder = services.AddControllersWithViews();

#if DEBUG
            builder.AddRazorRuntimeCompilation();
#endif

         services.AddSignalR();
         services.AddSingleton<SimConnector>();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SimConnector simconnector, IHostApplicationLifetime lifetime)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         simconnector.Connect();

         app.UseStaticFiles();
         app.UseRouting();

         lifetime.ApplicationStopping.Register(() =>
         {
            MessageWindow.GetWindow().Dispose();
         });

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapHub<WebSocketConnector>("/ws");
         });
      }
   }
}

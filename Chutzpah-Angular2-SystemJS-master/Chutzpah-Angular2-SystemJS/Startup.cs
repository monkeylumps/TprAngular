namespace Test
{
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public static void Main(string[] arguments) => WebApplication.Run<Startup>(arguments);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseIISPlatformHandler();
            application.UseStaticFiles();

            application.UseMvc(routes =>
            {
                routes.MapRoute("Index", "{*.}", new { Controller = "Home", Action = "Index" });
            });
        }
    }
}

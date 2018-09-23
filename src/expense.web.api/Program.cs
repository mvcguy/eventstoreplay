using expense.web.eventstore.EventSubscriber;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace expense.web.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        public static void StartSubscriber(IWebHost host)
        {
            /*
             * Issue when updating dababase using the migrations
             * Quote: (Ref: https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/)
             * "As stated in the migration docs move database related code out of the  Configure function of the  Startup class and into the  Main function. The following is the example of this from the docs."
             */

            using (var scope = host.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var eventSubscriber = provider.GetService<IEventStoreSubscriber>();
                if (eventSubscriber.IsStarted) return;
                eventSubscriber.Start(0);
            }
        }
    }
}

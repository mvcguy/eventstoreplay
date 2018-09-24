using System.Threading.Tasks;
using expense.web.eventstore.EventStoreSubscriber;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace expense.web.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            StartSubscriber(webHost);
            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
        }

        public static void StartSubscriber(IWebHost host)
        {
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

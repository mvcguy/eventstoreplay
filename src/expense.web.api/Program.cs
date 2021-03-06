﻿using System;
using System.Linq;
using System.Threading.Tasks;
using expense.web.api.Values.ReadModel;
using expense.web.api.Values.ReadModel.Schema;
using expense.web.eventstore.EventStoreSubscriber;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

                var subscriptionOptions = provider.GetService<IOptions<SubscriberOptions>>();
                Microsoft.Extensions.Logging.ILogger<Program> logger = provider.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();


                var readSourceName = subscriptionOptions.Value.TopicName;
                if (string.IsNullOrWhiteSpace(readSourceName))
                    throw new Exception("Event store subcriber cannot be started.");

                var repository = provider.GetService<IReadModelRepository<ReadPointer>>();
                var readPointer = repository.GetAll()
                    .FirstOrDefault(x => x.SourceName == readSourceName);
                if (readPointer == null)
                {
                    readPointer = new ReadPointer
                    {
                        SourceName = readSourceName,
                        Position = -1,
                        CreatedOn = DateTime.Now,
                        LastModifiedOn = DateTime.Now,
                        PublicId = Guid.NewGuid()
                    };
                    Task.Run(() =>
                    {
                        try
                        {
                            repository.AddAsync(readPointer);
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, e.Message);
                            throw;
                        }
                    }).Wait();
                }

                var eventSubscriber = provider.GetService<IEventStoreSubscriber>();
                if (eventSubscriber.IsStarted) return;

                var position = readPointer.Position < 0 ? null : readPointer.Position;

                eventSubscriber.Start(position);
            }
        }
    }
}

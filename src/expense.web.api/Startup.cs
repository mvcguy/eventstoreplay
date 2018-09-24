using System;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.ReadModel;
using expense.web.eventstore.EventStoreDataContext;
using expense.web.eventstore.EventStoreSubscriber;
using expense.web.eventstore.StoreConnection;
using EventStore.ClientAPI;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using IEventStoreLogger = EventStore.ClientAPI.ILogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;


namespace expense.web.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMediatR();
            services.Configure<SubscriberOptions>(options =>
            {
                options.TopicName = Configuration.GetSection("EventStore")["TopicName"];
                options.Connection = Configuration.GetSection("EventStore")["ConnectionString"];
            });


            services.AddValuesEventConsumer();

            services.AddSingleton<IEventStoreLogger, EventStoreLogger>();
            services.AddTransient(typeof(StoreContext<,>));
            services.AddTransient<IValuesRepository, ValuesRepository>();

            services.AddTransient(provider =>
            {
                // transient connection for repository operations
                var options = provider.GetService<IOptions<SubscriberOptions>>();
                IEventStoreConnection connection = ConnectionHelper.Create(options.Value.Connection, provider.GetService<IEventStoreLogger>(), TcpType.Normal);
                connection.ConnectAsync().Wait();
                return connection;
            });

            //read model
            services.AddScoped<IMongoDatabase>(x =>
            {
                var connectionString = Configuration["MongoDB:MongoContext:ConnectionString"];
                var databaseName = Configuration["MongoDB:MongoContext:DatabaseName"];
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                return database;
            });

            services.AddScoped(typeof(IReadModelRepository<>), typeof(MongoDbReadModelRepository<>));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }

    public static class ServiceExtensions
    {
        public static IServiceCollection AddValuesEventConsumer(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IEventStoreSubscriber>(provider =>
            {
                var options = provider.GetService<IOptions<SubscriberOptions>>();

                // we need a singleton connection
                IEventStoreConnection connection = ConnectionHelper.Create(options.Value.Connection, provider.GetService<IEventStoreLogger>(), TcpType.Normal);
                Microsoft.Extensions.Logging.ILogger<ValuesEventConsumer> logger = provider.GetService<Microsoft.Extensions.Logging.ILogger<ValuesEventConsumer>>();
                var eventConsumer = new ValuesEventConsumer(options, logger, connection);
                return eventConsumer;
            });
            return services;
        }
    }
}

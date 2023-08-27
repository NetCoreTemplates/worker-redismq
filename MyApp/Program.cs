using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;
using MyApp.ServiceModel;

namespace MyApp;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .UseServiceStack(new GenericAppHost(typeof(MyService).Assembly)
            {
                ConfigureAppHost = host =>
                {
                    var mqServer = host.Resolve<IMessageService>();
                    mqServer.RegisterHandler<Hello>(host.ExecuteMessage);
                }
            })
            .Run();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IRedisClientsManager>(
                    new RedisManagerPool(hostContext.Configuration.GetConnectionString("RedisMq")));
                services.AddSingleton<IMessageService>(c => new RedisMqServer(c.Resolve<IRedisClientsManager>()) {
                    DisablePublishingToOutq = true,
                });
                services.AddHostedService<MqWorker>();
            });
}

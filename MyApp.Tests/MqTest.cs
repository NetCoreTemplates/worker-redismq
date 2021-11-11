using NUnit.Framework;
using ServiceStack.Messaging;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;
using MyApp.ServiceModel;

namespace MyApp.Tests;

public class MqTest
{
    private IRedisClientsManager redisManager;
    private readonly IMessageFactory MqFactory;
    public MqTest()
    {
        redisManager = new RedisManagerPool("localhost:6379");
        MqFactory = new RedisTransientMessageFactory(redisManager);
    }

    [Test] // requires running Host MQ Server project
    public void Can_send_Request_Reply_message()
    {
        using (var mqClient = MqFactory.CreateMessageQueueClient())
        {
            var replyToMq = mqClient.GetTempQueueName();

            mqClient.Publish(new Message<Hello>(new Hello { Name = "MQ Worker" })
            {
                ReplyTo = replyToMq,
            });

            var responseMsg = mqClient.Get<HelloResponse>(replyToMq);
            mqClient.Ack(responseMsg);
            Assert.That(responseMsg.GetBody().Result, Is.EqualTo("Hello, MQ Worker!"));
        }
    }
}

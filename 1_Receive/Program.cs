using System;
using TIBCO.EMS;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Consumer (Receiver/Subscriber) started");
        new Program().Run();
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private void Run()
    {
        CreateClientTopicSubscriber("Owner LIKE '%HelloWorld%'"); // Pass "" for no message selector
    }

    #region EMS Client
    TopicConnection subscriberConnection;
    TopicSession subscriberSession;
    private void CreateClientTopicSubscriber(string messageSelector)
    {
        TopicConnectionFactory factory = new TIBCO.EMS.TopicConnectionFactory("localhost");
        subscriberConnection = factory.CreateTopicConnection("", "");  // Username, password
        subscriberConnection.Start();
        subscriberSession = subscriberConnection.CreateTopicSession(false, Session.AUTO_ACKNOWLEDGE);
        Topic clientTopic = subscriberSession.CreateTopic("GeneralTopic");
        TopicSubscriber clientTopicSubscriber = subscriberSession.CreateSubscriber(clientTopic, messageSelector, true);
        clientTopicSubscriber.MessageHandler += new EMSMessageHandler(test_MessageHandler);
    }

    void test_MessageHandler(object sender, EMSMessageEventArgs args)
    {
        Console.WriteLine(args.Message.GetType());
        Console.WriteLine("\nEMS Consumer received message: \n" + args.Message.ToString());
    }
    #endregion
}

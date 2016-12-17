using System;
using TIBCO.EMS;

class Program
{
    /// <summary>
    /// Run one or more Receivers or Subscribers
    /// </summary>
    TopicConnection subscriberConnection;
    TopicSession subscriberSession;

    static void Main(string[] args)
    {
        Console.WriteLine("1_Receive - Consumer (Subscriber) started");
        new Program().Run();
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private void Run()
    {
        CreateClientTopicSubscriber("Owner LIKE '%HelloWorld%'"); // Pass "" for no message selector
    }

    /// <summary>
    /// CreateClientTopicSubscriber
    /// </summary>
    /// <param name="messageSelector"></param>
    private void CreateClientTopicSubscriber(string messageSelector)
    {
        var topicname = "GeneralTopic";
        Console.WriteLine("Createing a topic we can publish to: " + topicname);

        TopicConnectionFactory factory = new TopicConnectionFactory("localhost");
        subscriberConnection = factory.CreateTopicConnection("", "");  // Username, password
        subscriberConnection.Start();

        subscriberSession = subscriberConnection.CreateTopicSession(false, Session.AUTO_ACKNOWLEDGE);
        Topic clientTopic = subscriberSession.CreateTopic(topicname);

        // using message selector to demostrate we can subscribe to a sepecific topic by using the
        // textMessage.SetStringProperty("Owner", "HelloWorld");
        // If the messageSelector does not match not message will be received:  "Owner LIKE '%HelloWorld%'"
        TopicSubscriber clientTopicSubscriber = subscriberSession.CreateSubscriber(clientTopic, messageSelector, true);

        //Wireup an event handler for async message consumption
        clientTopicSubscriber.MessageHandler += new EMSMessageHandler(event_MessageHandler);
    }

    void event_MessageHandler(object sender, EMSMessageEventArgs args)
    {
        Console.WriteLine(args.Message.GetType());
        Console.WriteLine("\n1_Receive received message: \n" + args.Message.ToString());
    }
}

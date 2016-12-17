using System;
using TIBCO.EMS;

class Program
{
    /// <summary>
    /// For this example make sure you run 1_Receive.exe at least once.  You can start up more than
    /// one instance of 1_Receive.exe to see the Publisher (1_Send.exe) publish a message to all of
    /// the running receivers (Subscribers).
    /// </summary>
    TopicConnection publisherConnection;
    TopicSession publisherSession;
    TopicPublisher emsServerPublisher;

    static void Main(string[] args)
    {
        Console.WriteLine("1_Send - Producer (Publisher) started");
        new Program().Run();
        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    }

    private void Run()
    {
        CreateEMSServerTopicPublisher();
        EMSServerPublishThisMessage("Hello World", "Owner", "HelloWorld");
    }

    /// <summary>
    /// CreateEMSServerTopicPublisher creates the Topic we will need to publish to
    /// and binds the Topic to a session and creates the publisher.
    /// </summary>
    private void CreateEMSServerTopicPublisher()
    {
        var topicname = "GeneralTopic";
        Console.WriteLine("Createing a topic we can publish to: " + topicname);

        TopicConnectionFactory factory = new TopicConnectionFactory("localhost");
        publisherConnection = factory.CreateTopicConnection("", ""); // Username, password blank for dev instance
        publisherSession = publisherConnection.CreateTopicSession(false, Session.AUTO_ACKNOWLEDGE);
        Topic generalTopic = publisherSession.CreateTopic("GeneralTopic");
        emsServerPublisher = publisherSession.CreatePublisher(generalTopic);

        publisherConnection.Start();
    }

    /// <summary>
    /// EMSServerPublishThisMessage publishes the message to EMS
    /// </summary>
    /// <param name="message"></param>
    /// <param name="propertyName"></param>
    /// <param name="propertyValue"></param>
    internal void EMSServerPublishThisMessage(string message, string propertyName, string propertyValue)
    {
        TextMessage textMessage = publisherSession.CreateTextMessage();
        textMessage.Text = message;
        textMessage.SetStringProperty(propertyName, propertyValue);
        emsServerPublisher.Publish(textMessage);
        Console.WriteLine("1_Send published message: " + message);
    }
}

using System;
using TIBCO.EMS;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Producer started");
        new Program().Run();
        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    }

    private void Run()
    {
        CreateEMSServerTopicPublisher();
        EMSServerPublishThisMessage("Hello World", "Owner", "HelloWorld");
    }

    TopicConnection publisherConnection;
    TopicSession publisherSession;
    TopicPublisher emsServerPublisher;
    private void CreateEMSServerTopicPublisher()
    {
        TopicConnectionFactory factory = new TIBCO.EMS.TopicConnectionFactory("localhost");
        publisherConnection = factory.CreateTopicConnection("", ""); // Username, password
        publisherSession = publisherConnection.CreateTopicSession(false, Session.AUTO_ACKNOWLEDGE);
        Topic generalTopic = publisherSession.CreateTopic("GeneralTopic");
        emsServerPublisher = publisherSession.CreatePublisher(generalTopic);

        publisherConnection.Start();
    }

    internal void EMSServerPublishThisMessage(string message, string propertyName, string propertyValue)
    {
        TextMessage textMessage = publisherSession.CreateTextMessage();
        textMessage.Text = message;
        textMessage.SetStringProperty(propertyName, propertyValue);
        emsServerPublisher.Publish(textMessage);
        Console.WriteLine("EMS Publisher published TEXT message: " + message);
    }
}


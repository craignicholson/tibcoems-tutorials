using System;
using TIBCO.EMS;

class Program
{
    string serverUrl = "localhost";
    string userName = null;
    string password = null;
    string topicName = "GeneralTopic";
    string clientID = "Craig.Nicholson";
    string durableName = "helloworld.subscriber";

    static void Main(string[] args)
    {
        new Program().DurableExample(args) ;
        Console.ReadLine();
    }

    private void DurableExample(string[] args)
    {
        try
        {
            ConnectionFactory factory = new TIBCO.EMS.ConnectionFactory(serverUrl);
            Connection connection = factory.CreateConnection(userName, password);

            // if clientID is specified we must set it right here
            if (clientID != null)
                connection.ClientID = clientID;

            Session session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);

            bool unsubscribe = false;
            if (unsubscribe)
            {
                Console.WriteLine("Unsubscribing durable subscriber " + durableName);
                session.Unsubscribe(durableName);
                Console.WriteLine("Successfully unsubscribed " + durableName);
                connection.Close();
                return;
            }

            Console.WriteLine("Subscribing to topic: " + topicName);

            // Use createTopic() to enable subscriptions to dynamic topics.
            Topic topic = session.CreateTopic(topicName);
            TopicSubscriber subscriber = session.CreateDurableSubscriber(topic, durableName);

            connection.Start();

            // read topic messages
            while (true)
            {
                Message message = subscriber.Receive();
                if (message == null)
                    break;
                Console.WriteLine("\nReceived message: " + message);
            }
            connection.Close();
        }
        catch (EMSException e)
        {
            Console.Error.WriteLine("Exception in 1_ReceiveDurable: " + e.Message);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(0);
        }
    }
}

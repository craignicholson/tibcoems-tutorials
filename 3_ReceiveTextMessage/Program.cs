using System;
using System.Threading;
using TIBCO.EMS;

class Program
{
    /// <summary>
    /// Point-to-point messaging has one msgProducer and one consumer per message. 
    /// This style of messaging uses a queue to store messages until they are received. 
    /// The message msgProducer sends the message to the queue; the message consumer retrieves 
    /// messages from the queue and sends acknowledgement that the message was received. 
    /// </summary>
    string serverUrl = "localhost";
    string userName = null;
    string password = null;
    string queueName = "EventQ";
    int ackMode = Session.AUTO_ACKNOWLEDGE;

    public static void Main(string[] args)
    {
        Console.WriteLine("ReceiveTextMessage Consumer started");
        new Program().Run(args);
        //Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    public void Run(string[] args)
    {
        try
        {
            Console.WriteLine("Server " + ((serverUrl != null) ? serverUrl : "localhost"));
            Console.WriteLine("User " + ((userName != null) ? userName : "(null)"));
            Console.WriteLine("Queue " + queueName);

            ConnectionFactory factory = new TIBCO.EMS.ConnectionFactory(serverUrl);
            Connection connection = factory.CreateConnection(userName, password);
            Session session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);

            Destination queue = session.CreateQueue(queueName);
            MessageProducer msgProducer = session.CreateProducer(queue);

            // Start the Connection
            // Don't I need a connection.Close some where?
            connection.Start();

            MessageConsumer consumer = session.CreateConsumer(queue);
            Console.WriteLine("Waiting for messsages in queue " + queueName);
            consumer.MessageHandler += new EMSMessageHandler(event_MessageHandler);
        }
        catch (EMSException e)
        {
            Console.Error.WriteLine("Exception in ReceiveTextMessage: " + e.Message);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(0);
        }
        catch (ThreadInterruptedException e)
        {
            Console.Error.WriteLine("Exception in ReceiveTextMessage: " + e.Message);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// event_MessageHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void event_MessageHandler(object sender, EMSMessageEventArgs args)
    {
        Console.WriteLine("Received message: " + args.Message);

        // Acknowledge we received the message.  If the message did not get written to disk
        // we want to not acknowledge the message was received.
        if (ackMode == Session.CLIENT_ACKNOWLEDGE ||
            ackMode == Session.EXPLICIT_CLIENT_ACKNOWLEDGE ||
            ackMode == Session.EXPLICIT_CLIENT_DUPS_OK_ACKNOWLEDGE)
            args.Message.Acknowledge();
    }
}
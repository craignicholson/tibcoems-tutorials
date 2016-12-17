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
    string queueName = "HelloWorldQ";
    int ackMode = Session.AUTO_ACKNOWLEDGE;

    public static void Main(string[] args)
    {
        Console.WriteLine("ReceiveHelloWorld Point-to-Point Consumer started");
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
            Console.Error.WriteLine("Exception in ReceiveHelloWorld: " + e.Message);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(0);
        }
        catch (ThreadInterruptedException e)
        {
            Console.Error.WriteLine("Exception in ReceiveHelloWorld: " + e.Message);
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
        // Since args.Message is generic message we need to box into TextMessage
        // which allows us access to the message.Text content.
        TextMessage message = (TextMessage)args.Message;
        string msg = message.Text;
        Console.Write(DateTime.Now.ToLongTimeString() + "\t");
        Console.WriteLine("Received message: " + msg);

        // Acknowledge we received the message.  If the message did not get written to disk
        // we want to not acknowledge the message was received.
        if (ackMode == Session.CLIENT_ACKNOWLEDGE ||
            ackMode == Session.EXPLICIT_CLIENT_ACKNOWLEDGE ||
            ackMode == Session.EXPLICIT_CLIENT_DUPS_OK_ACKNOWLEDGE)
            args.Message.Acknowledge();
    }
}
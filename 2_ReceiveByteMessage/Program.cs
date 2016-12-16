using System;
using System.IO;
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
    string queueName = "ExportQ";
    int ackMode = Session.AUTO_ACKNOWLEDGE;

    public static void Main(string[] args)
    {
        Console.WriteLine("2_ReceiveByteMessage Point-to-Point Consumer started");
        new Program().Run(args);
        //Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    public void Run(string[] args)
    {
        Console.WriteLine("Server " + ((serverUrl != null) ? serverUrl : "localhost"));
        Console.WriteLine("User " + ((userName != null) ? userName : "(null)"));
        Console.WriteLine("Queue " + queueName);

        try
        {
            ConnectionFactory factory = new ConnectionFactory(serverUrl);
            Connection connection = factory.CreateConnection(userName, password);
            Session session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);

            // Used .Queue here... but used Destination in SendByteMessage, is there a difference?
            // Testing the theory
            //TIBCO.EMS.Queue queue = session.CreateQueue(queueName);
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
            Console.Error.WriteLine("Exception in ReceiveByteMessage: " + e.Message);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(0);
        }
        catch (ThreadInterruptedException e)
        {
            Console.Error.WriteLine("Exception in ReceiveByteMessage: " + e.Message);
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
        if (args.Message is BytesMessage)
        {
            Console.WriteLine("Writing file to disk\n");
            var filename = args.Message.GetStringProperty("FILE_NAME");
            var filesize = args.Message.GetStringProperty("FILE_SIZE");
            byte[] data = null;
            BytesMessage bm = (BytesMessage)args.Message;
            data = new byte[(int)bm.BodyLength]; // I could send in FILE_SIZE here.

            // fill the data variable with the bytes in the ByteMessage
            bm.ReadBytes(data);

            try
            {
                File.WriteAllBytes(filename + ".xml", data);
            }
            catch(IOException ex)
            {
                Console.Error.WriteLine(ex.StackTrace);
                Environment.Exit(0);
            }
        }

        // Acknowledge we received the message.  If the message did not get written to disk
        // we want to not acknowledge the message was received.
        if (ackMode == Session.CLIENT_ACKNOWLEDGE ||
            ackMode == Session.EXPLICIT_CLIENT_ACKNOWLEDGE ||
            ackMode == Session.EXPLICIT_CLIENT_DUPS_OK_ACKNOWLEDGE)
           args.Message.Acknowledge();
    }
}
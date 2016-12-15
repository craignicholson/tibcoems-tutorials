using System;
using System.Collections;
using System.IO;
using System.Threading;
using TIBCO.EMS;

class Program
{

    string serverUrl = "localhost";
    string userName = null;
    string password = null;
    string queueName = "ExportQ";
    int ackMode = Session.AUTO_ACKNOWLEDGE;

    public static void Main(string[] args)
    {
        new Program().Run(args);
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    public void Run(string[] args)
    {
        try
        {
            //tibemsUtilities.initSSLParams(serverUrl, args);
        }
        catch (Exception e)
        {
            System.Console.WriteLine("Exception: " + e.Message);
            System.Console.WriteLine(e.StackTrace);
            System.Environment.Exit(-1);
        }

        Console.WriteLine("\n------------------------------------------------------------------------");
        Console.WriteLine("csBrowser SAMPLE");
        Console.WriteLine("------------------------------------------------------------------------");
        Console.WriteLine("Server....................... " + ((serverUrl != null) ? serverUrl : "localhost"));
        Console.WriteLine("User......................... " + ((userName != null) ? userName : "(null)"));
        Console.WriteLine("Queue........................ " + queueName);
        Console.WriteLine("------------------------------------------------------------------------\n");

        try
        {
            ConnectionFactory factory = new TIBCO.EMS.ConnectionFactory(serverUrl);

            Connection connection = factory.CreateConnection(userName, password);
            Session session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);
            TIBCO.EMS.Queue queue = session.CreateQueue(queueName);
            MessageProducer producer = session.CreateProducer(queue);
            Message message = null;

            connection.Start();

            MessageConsumer consumer = session.CreateConsumer(queue);
            Console.WriteLine("Polling the queue " + queueName);

            // read queue until empty
            // read messages
            while (true)
            {
                // receive the message
                message = consumer.Receive();
                if (message == null)
                    break;

                Console.WriteLine("Received message: " + message);
                if (message is BytesMessage)
                {
                    Console.WriteLine("MessageType : " + message.GetType());
                    //Message msg = args.Message;
                    var filename = message.GetStringProperty("FILE_NAME");
                    var filesize = message.GetStringProperty("FILE_SIZE");
                    byte[] data = null;
                    BytesMessage bm = (BytesMessage)message;
                    data = new byte[(int)bm.BodyLength]; // I could sub in FILE_SIZE here.
                    bm.ReadBytes(data);
                    //Console.WriteLine(Encoding.UTF8.GetString(data));
                    File.WriteAllBytes(filename + ".xml", data);
                }
                // Check to see if we need to ACK, ACK will allow the message to be removed from the queue.
                if (ackMode == Session.CLIENT_ACKNOWLEDGE ||
                    ackMode == Session.EXPLICIT_CLIENT_ACKNOWLEDGE ||
                    ackMode == Session.EXPLICIT_CLIENT_DUPS_OK_ACKNOWLEDGE)
                    message.Acknowledge();
            }
            connection.Close();
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
}
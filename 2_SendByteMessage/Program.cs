using System;
using System.IO;
using TIBCO.EMS;

class Program
{
    string queueName = "ExportQ";
    Connection connection = null;
    Session session = null;
    MessageProducer msgProducer = null;
    Destination queue = null;

    static void Main(string[] args)
    {
        Console.WriteLine("2_SendByteMessage Server");
        new Program().Run();
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private void Run()
    {
        try
        {
            BytesMessage msg;
            Console.WriteLine("Publishing to queue '" + queueName + "'\n");

            ConnectionFactory factory = new ConnectionFactory("localhost");
            connection = factory.CreateConnection("", "");

            // create the session
            session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);
            queue = session.CreateQueue(queueName);

            //create the producer
            msgProducer = session.CreateProducer(null);

            byte[] file = File.ReadAllBytes("Nlog.xml");
            msg = session.CreateBytesMessage();
            msg.WriteBytes(file);
            msg.MsgType = "Byte";

            //Add additional properties - which function as key value pairs
            msg.SetStringProperty("FILE_SIZE", file.Length.ToString());
            msg.SetStringProperty("FILE_NAME", Guid.NewGuid().ToString());

            //compress the data
            msg.SetBooleanProperty("JMS_TIBCO_COMPRESS", true);

            //publish the message
            msgProducer.Send(queue, msg);
            Console.WriteLine("Published message: " + msg.ToString());
        }
        catch (EMSException e)
        {
            Console.Error.WriteLine("Exception in 2_SendByteMessage: " + e.Message);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(-1);
        }
    }
}
using System;
using System.IO;
using TIBCO.EMS;


class Program
{
    string serverUrl = null;
    string userName = null;
    string password = null;
    string name = "ExportQ";
    string clientID = null;
    string durableName = "subscriber";

    bool useTopic = true;
    bool useAsync = false;

    Connection connection = null;
    Session session = null;
    MessageProducer msgProducer = null;
    Destination destination = null;

    //EMSCompletionListener completionListener = null;


    static void Main(string[] args)
    {
        Console.WriteLine("SendByteMessage Server (Sender/Publisher) started - BYTE MESSAGE");
        new Program().Run();
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private void Run()
    {
        try
        {
            BytesMessage msg;
            Console.WriteLine("Publishing to destination '" + name + "'\n");

            ConnectionFactory factory = new TIBCO.EMS.ConnectionFactory("localhost");
            connection = factory.CreateConnection("", "");

            // create the session
            session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);
            destination = session.CreateQueue(name);

            //create the producer
            msgProducer = session.CreateProducer(null);

            byte[] file = File.ReadAllBytes("Nlog.xml");
            msg = session.CreateBytesMessage();
            msg.WriteBytes(file);
            msg.MsgType = "Byte";

            //Add special properties
            msg.SetStringProperty("FILE_SIZE", file.Length.ToString());
            msg.SetStringProperty("FILE_NAME", Guid.NewGuid().ToString());

            //compress
            msg.SetBooleanProperty("JMS_TIBCO_COMPRESS", true);

            //publish the message
            msgProducer.Send(destination, msg);
            Console.WriteLine("Published message: " + msg.ToString());
        }
        catch (EMSException e)
        {
            Console.Error.WriteLine("Exception in csMsgProducer: " + e.Message);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(-1);
        }
    }
}
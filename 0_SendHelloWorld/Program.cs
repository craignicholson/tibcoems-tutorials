using System;
using TIBCO.EMS;

class Program
{
    string queueName = "HelloWorldQ";
    Connection connection = null;
    Session session = null;
    MessageProducer msgProducer = null;
    Destination queue = null;

    static void Main(string[] args)
    {
        Console.WriteLine("SendHelloWorld Producer");
        new Program().Run();
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private void Run()
    {
        try
        {
            TextMessage msg;
            Console.WriteLine("Publishing to queue '" + queueName + "'\n");

            ConnectionFactory factory = new TIBCO.EMS.ConnectionFactory("localhost");
            connection = factory.CreateConnection("", "");

            // create the session
            session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);
            queue = session.CreateQueue(queueName);

            //create the producer
            msgProducer = session.CreateProducer(null);

            msg = session.CreateTextMessage();

            // load the txt
            msg.Text = "Hello World";

            //compress
            msg.SetBooleanProperty("JMS_TIBCO_COMPRESS", true);

            //publish the message
            msgProducer.Send(queue, msg);
            Console.WriteLine("Published message: " + msg.ToString());
        }
        catch (EMSException e)
        {
            Console.Error.WriteLine("Exception in SendHelloWorld : " + e.Message);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(-1);
        }
    }
}
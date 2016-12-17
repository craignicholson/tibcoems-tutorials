using System;
using System.Threading;
using TIBCO.EMS;

class Program
{
    string queueName = "TasksQ";
    Connection connection = null;
    Session session = null;
    MessageProducer msgProducer = null;
    Destination queue = null;

    static void Main(string[] args)
    {
        Console.WriteLine("Tasks Producer");
        new Program().Run();
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private void Run()
    {
        Console.WriteLine("Publishing to queue '" + queueName + "'\n");
        int tasks = 1000;

        // Generate random numbers, more interesting than just i being incremented
        Random rnd = new Random();
        try
        {
            for (int i=0 ; i < tasks; i++) {
                TextMessage msg;

                ConnectionFactory factory = new TIBCO.EMS.ConnectionFactory("localhost");
                connection = factory.CreateConnection("", "");

                // create the session
                session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);
                queue = session.CreateQueue(queueName);

                //create the producer
                msgProducer = session.CreateProducer(null);

                msg = session.CreateTextMessage();
                var number = rnd.Next(1000,1000000);

                // update console
                Console.WriteLine("Number : " + number);

                // load the txt - which is a number we want to now the primes
                // note: receiver uses Convert.ToInt32 so we need to only pass in valid number.
                msg.Text = number.ToString();

                //Another options is to use a property
                msg.SetIntProperty("number", number);

                //compress
                //msg.SetBooleanProperty("JMS_TIBCO_COMPRESS", true);

                //publish the message
                msgProducer.Send(queue, msg);
                Thread.Sleep(100);
            }
            Console.WriteLine("Tasks Sent : " + tasks.ToString());
        }
        catch (EMSException e)
        {
            Console.Error.WriteLine("Exception in Tasks : " + e.Message);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(-1);
        }
    }
}
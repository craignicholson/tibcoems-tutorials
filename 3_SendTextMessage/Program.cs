using System;
using System.IO;
using TIBCO.EMS;

class Program
{
    string queueName = "EventQ";
    Connection connection = null;
    Session session = null;
    MessageProducer msgProducer = null;
    Destination queue = null;

    static void Main(string[] args)
    {
        Console.WriteLine("SendTextMessage Server");
        new Program().Run();
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private void Run()
    {
        try
        {
            TextMessage msg;
            Console.WriteLine("Publishing to destination queue '" + queueName + "'\n");

            ConnectionFactory factory = new TIBCO.EMS.ConnectionFactory("localhost");
            connection = factory.CreateConnection("", "");

            // create the session
            session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);
            queue = session.CreateQueue(queueName);

            //create the producer
            msgProducer = session.CreateProducer(null);

            string xml = @"<Event Id='15040' Code='C1219_DEMAND_RESET' Severity='EVENT_SEVERITY_INFORMATION' Name='C1219 Demand Reset' Class='DemandResetOccurred'>
                                <Description> C1219 Demand Reset occurred </ Description > < Message > Demand Reset occurred for meter { 0}.</ Message>
                                <Parameter Index = '0' Name = 'sourceEmitter' Type = 'Device'/> 
                                <MetaInfo>
                                    <MeterSource> Std table 76.table proc number 20 </MeterSource>
                                </MetaInfo>
                          </Event> ";

            msg = session.CreateTextMessage();

            //Add special properties
            msg.SetStringProperty("NUMBER_OF_EVENTS", 1.ToString());
            
            // load the txt or xml in our use case
            msg.Text = xml;

            //compress
            msg.SetBooleanProperty("JMS_TIBCO_COMPRESS", true);

            //publish the message
            msgProducer.Send(queue, msg);
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
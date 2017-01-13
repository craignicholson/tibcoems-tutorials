using System;
using TIBCO.EMS;
using System.Collections;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public class emsSSLSampleClient
{
    // *-server ssl://localhost:7243 
    // -ssl_target_hostname <hostname>
    // -ssl_cert_store_location currentuser 
    // -ssl_cert_store_name My 
    // -ssl_cert_name "CN=client-sample-cert"
    string serverUrl = "ssl://localhost:7243";
    string userName = null;
    string password = "password";
    string topicName = "topic.sample";

    // SSL options - https://msdn.microsoft.com/en-us/library/windows/desktop/aa388136(v=vs.85).aspx
    // 
    bool ssl_trace = true;
    string ssl_target_hostname = "SampleAuthority"; //https://docs.tibco.com/pub/ems/8.2.1/doc/html/TIB_ems_api_reference/api/dotnetdoc/html/class_t_i_b_c_o_1_1_e_m_s_1_1_e_m_s_s_s_l.html#a177f8c2f67a2b8b759f7176096907575
    string ssl_cert_store_location = "localmachine"; //localmachine
    string ssl_cert_store_name = "My";
    string ssl_cert_name = "client-sample-cert";

    public emsSSLSampleClient(string[] args)
    {
        if (topicName == null)
        {
            System.Console.WriteLine("Error: must specify topic name");
            usage();
        }

        System.Console.WriteLine("Global SSL parameters sample with Microsoft Certificate Store.");

        try
        {
            EMSSSLSystemStoreInfo storeInfo = new EMSSSLSystemStoreInfo();
            //EMSSSLFileStoreInfo storeInfo = new EMSSSLFileStoreInfo();

            // set trace for client-side operations, loading of certificates
            // and other
            if (ssl_trace)
            {
                EMSSSL.SetClientTracer(new System.IO.StreamWriter(System.Console.OpenStandardError()));
            }

            // set target host name in the sertificate if specified
            if (ssl_target_hostname != null)
            {
                EMSSSL.SetTargetHostName(ssl_target_hostname);
            }

            if (ssl_cert_store_location != null)
            {
                if (ssl_cert_store_location.Equals("currentuser"))
                {
                    storeInfo.SetCertificateStoreLocation(StoreLocation.CurrentUser);
                }
                else if (ssl_cert_store_location.Equals("localmachine"))
                {
                    storeInfo.SetCertificateStoreLocation(StoreLocation.LocalMachine);
                }
            }

            if (ssl_cert_store_name != null)
            {
                storeInfo.SetCertificateStoreName(ssl_cert_store_name);
            }
            if (ssl_cert_name != null)
            {
                storeInfo.SetCertificateNameAsFullSubjectDN(ssl_cert_name);
            }

            EMSSSL.SetCertificateStoreType(EMSSSLStoreType.EMSSSL_STORE_TYPE_SYSTEM, storeInfo);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.StackTrace);

            if (e is EMSException)
            {
                EMSException je = (EMSException)e;

                if (je.LinkedException != null)
                {
                    System.Console.WriteLine("##### Linked Exception:");
                    System.Console.WriteLine(je.LinkedException.StackTrace);
                }
            }
            //System.Environment.Exit(-1);
        }

        try
        {
            
            ConnectionFactory factory = new ConnectionFactory(serverUrl);
            //factory.SetSSLTrace(true);
            factory.SetConnAttemptCount(1);
            factory.SetConnAttemptDelay(1000);
            factory.SetConnAttemptTimeout(1000);

            Connection connection = factory.CreateConnection(userName, password);

            Session session = connection.CreateSession(false, TIBCO.EMS.SessionMode.AutoAcknowledge);

            Topic topic = session.CreateTopic(topicName);

            MessageProducer publisher = session.CreateProducer(topic);
            MessageConsumer subscriber = session.CreateConsumer(topic);

            connection.Start();

            MapMessage message = session.CreateMapMessage();
            message.SetStringProperty("field", "SSL message");

            for (int i = 0; i < 3; i++)
            {
                publisher.Send(message);
                System.Console.WriteLine("\nPublished message: " + message);

                /* read same message back */
                message = (MapMessage)subscriber.Receive();
                if (message == null)
                    System.Console.WriteLine("\nCould not receive message");
                else
                    System.Console.WriteLine("\nReceived message: " + message);

                try
                {
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception) { }
            }

            connection.Close();
        }
        catch (EMSException e)
        {
            System.Console.WriteLine("##### Exception Connect:" + e.Message);
            System.Console.WriteLine(e.StackTrace);

            if (e.LinkedException != null)
            {
                System.Console.WriteLine("##### Linked Exception error msg:" + e.LinkedException.Message);
                System.Console.WriteLine("##### Linked Exception:");
                System.Console.WriteLine(e.LinkedException.StackTrace);
            }
            //System.Environment.Exit(-1);
        }
    }

    public static void Main(string[] args)
    {
        emsSSLSampleClient t = new emsSSLSampleClient(args);
        Console.ReadLine();
    }

    void usage()
    {
        System.Console.WriteLine("\nUsage:EMSSSLGlobal [options]");
        System.Console.WriteLine("");
        System.Console.WriteLine("   where options are:");
        System.Console.WriteLine("");
        System.Console.WriteLine(" -server        <server URL>   - EMS server URL, default is ssl://localhost:7243");
        System.Console.WriteLine(" -user          <user name>    - user name, default is null");
        System.Console.WriteLine(" -password      <password>     - password, default is null");
        System.Console.WriteLine(" -topic         <topic-name>   - topic name, default is \"topic.sample\"");
        System.Console.WriteLine("");
        System.Console.WriteLine("  SSL options:");
        System.Console.WriteLine("");
        System.Console.WriteLine(" -ssl_trace                         - trace SSL initialization");
        System.Console.WriteLine(" -ssl_target_hostname  <host-name>  - name in the server certificate");
        System.Console.WriteLine(" -ssl_cert_store_location    <location>   - system store where location, currentuser or localmachine");
        System.Console.WriteLine(" -ssl_cert_store_name  <store_name> - name of the store at the store location");
        System.Console.WriteLine(" -ssl_cert_name        <cert-name>  - name of the certificate");
        //System.Environment.Exit(-1);
    }
}
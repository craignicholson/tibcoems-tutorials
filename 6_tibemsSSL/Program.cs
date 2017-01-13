/* 
 * Copyright (c) 2001-2013 TIBCO Software Inc. 
 * All rights reserved.
 * For more information, please contact:
 * TIBCO Software Inc., Palo Alto, California, USA
 * 
 * $Id: tibemsSSL.cs 66337 2013-03-28 16:38:23Z $
 * 
 */

/*
 * This is a simple sample of SSL connectivity for
 * EMS .NET clients.  It demonstrates ssl-based client/server communications.
 *
 * This sample establishes an SSL connection, publishes and
 * receives back three messages.
 *
 *      -server     Server URL passed to the ConnectionFactory
 *                  constructor.
 *                  If not specified this sample assumes a
 *                  serverUrl of "ssl://localhost:7243"
 *
 *
 * Example usage:
 *
 *  tibemsSSL -server ssl://localhost:7243
 *
 * This sample can also demonstrate full use of SSL with the mutual
 * authentication. The following command line will cause the client
 * to verify the server certificate and also establishes the client
 * identity which will be verified by the server. In the sample
 * command line below the client-side SSL trace is being enabled,
 * if you also running the server with -ssl_debug_trace parameter
 * you will see the entire authentication trace:
 *
 * tibemsSSL -ssl_trace
 *                -ssl_trusted ../certs/server_root.cert.pem
 *                -ssl_target_hostname server
 *                -ssl_identity ../certs/client_identity.p12
 *                -ssl_password password
 */

using System;
using System.Collections;
using TIBCO.EMS;

/*   References
 *   https://docs.tibco.com/pub/enterprise_message_service/8.1.0/doc/html/tib_ems_api_reference/api/javadoc/com/tibco/tibjms/TibjmsSSL.html
 *   https://tibco4all.com/2016/02/02/how-to-configure-ssl-in-the-ems-server/
 *   
 *  ../certs/readme.txt
 *****************************************************************
 * All PKCS8, PKCS12 and other files encrypted with password:    *
 *                                                               *
 *                     "password"                                *
 *                                                               *
 *****************************************************************
 *  
 *  Issues - Errors
 *  The remote certificate is invalid according to the validation procedure
 *  C:\tibco\ems\8.3\bin>tibemsd.exe -config "C:\ProgramData\TIBCO_HOME\tibco\cfgmgmt\ems\data\tibemsd.conf" -ssl_debug_trace
 *  
 *  See tibemsd.conf in this project for the conf used.
 *  
 * SSL sample.
 * 2017-01-12 13:44:38.532 [8 ] [TIBCO EMS]: [.NET] [SSL] Creating the SSLStream ..
 * 2017-01-12 13:44:38.537 [8 ] [TIBCO EMS]: [.NET] [SSL] Using store file = C:/tibco/ems/8.3/samples/certs/client_identity.p12
 * 2017-01-12 13:44:38.569 [8 ] [TIBCO EMS]: [.NET] [SSL] -------------------------
 * 2017-01-12 13:44:38.569 [8 ] [TIBCO EMS]: [.NET] [SSL] Client Certificate:
 * 2017-01-12 13:44:38.569 [8 ] [TIBCO EMS]: [.NET] [SSL]            Subject: E=client@testcompany.com, CN=client, OU=client Unit, O=Test Company, L=us-english, S=California, C=US
 * 2017-01-12 13:44:38.569 [8 ] [TIBCO EMS]: [.NET] [SSL]             Issuer: E=client_root@testcompany.com, CN=client_root, OU=client_root Unit, O=Test Company, L=us-english, S=California, C=US
 * 2017-01-12 13:44:38.569 [8 ] [TIBCO EMS]: [.NET] [SSL]         Not Before: 2/21/2013 6:28:31 PM
 * 2017-01-12 13:44:38.569 [8 ] [TIBCO EMS]: [.NET] [SSL]          Not After: 2/19/2023 6:28:31 PM
 * 2017-01-12 13:44:38.569 [8 ] [TIBCO EMS]: [.NET] [SSL] -------------------------
 * 2017-01-12 13:44:38.569 [8 ] [TIBCO EMS]: [.NET] [SSL] Authenticating the client
 * 2017-01-12 13:44:38.634 [8 ] [TIBCO EMS]: [.NET] [SSL] ###########################
 * 2017-01-12 13:44:38.634 [8 ] [TIBCO EMS]: [.NET] [SSL] Server Certificate:
 * 2017-01-12 13:44:38.634 [8 ] [TIBCO EMS]: [.NET] [SSL]            Subject: E=server_root@testcompany.com, CN=server_root, OU=server_root Unit, O=Test Company, L=us-english, S=California, C=US
 * 2017-01-12 13:44:38.634 [8 ] [TIBCO EMS]: [.NET] [SSL]             Issuer: E=server_root@testcompany.com, CN=server_root, OU=server_root Unit, O=Test Company, L=us-english, S=California, C=US
 * 2017-01-12 13:44:38.634 [8 ] [TIBCO EMS]: [.NET] [SSL]         Not Before: 2/21/2013 6:28:29 PM
 * 2017-01-12 13:44:38.634 [8 ] [TIBCO EMS]: [.NET] [SSL]          Not After: 2/19/2023 6:28:29 PM
 * 2017-01-12 13:44:38.635 [8 ] [TIBCO EMS]: [.NET] [SSL] ###########################
 * 2017-01-12 13:44:39.140 [8 ] [TIBCO EMS]: [.NET] [SSL] Creating the SSLStream ..
 * 2017-01-12 13:44:39.140 [8 ] [TIBCO EMS]: [.NET] [SSL] Using store file = C:/tibco/ems/8.3/samples/certs/client_identity.p12
 * 2017-01-12 13:44:39.171 [8 ] [TIBCO EMS]: [.NET] [SSL] -------------------------
 * 2017-01-12 13:44:39.189 [8 ] [TIBCO EMS]: [.NET] [SSL] Client Certificate:
 * 2017-01-12 13:44:39.189 [8 ] [TIBCO EMS]: [.NET] [SSL]            Subject: E=client@testcompany.com, CN=client, OU=client Unit, O=Test Company, L=us-english, S=California, C=US
 * 2017-01-12 13:44:39.191 [8 ] [TIBCO EMS]: [.NET] [SSL]             Issuer: E=client_root@testcompany.com, CN=client_root, OU=client_root Unit, O=Test Company, L=us-english, S=California, C=US
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL]         Not Before: 2/21/2013 6:28:31 PM
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL]          Not After: 2/19/2023 6:28:31 PM
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL] -------------------------
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL] Authenticating the client
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL] ###########################
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL] Server Certificate:
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL]            Subject: E=server_root@testcompany.com, CN=server_root, OU=server_root Unit, O=Test Company, L=us-english, S=California, C=US
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL]             Issuer: E=server_root@testcompany.com, CN=server_root, OU=server_root Unit, O=Test Company, L=us-english, S=California, C=US
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL]         Not Before: 2/21/2013 6:28:29 PM
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL]          Not After: 2/19/2023 6:28:29 PM
 * 2017-01-12 13:44:39.194 [8 ] [TIBCO EMS]: [.NET] [SSL] ###########################
 * ERROR MSG: Failed to connect via SSL to [ssl://localhost:7243]: Failed to connect via SSL to [ssl://localhost:7243]: The remote certificate is invalid according to the validation procedure.
 *    at TIBCO.EMS.CFImpl._CreateConnection(String userName, String password, Boolean xa)
 *    at TIBCO.EMS.ConnectionFactory.CreateConnection(String userName, String password)
 *    at tibemsSSL..ctor(String[] args) in C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\tibcoems-tutorials\TibcoEMS-tutorials\6_tibemsSSL\Program.cs:line 158
 * Linked Exception Error Msg: Failed to connect via SSL to [ssl://localhost:7243]: Failed to connect via SSL to [ssl://localhost:7243]: The remote certificate is invalid according to the validation procedure.
 * ##### Linked Exception:
 *   at TIBCO.EMS.LinkSSL.Connect(URL url)
 * 
 * 
 *  A call to SSPI failed
 */
public class tibemsSSL
{
    string serverUrl = "ssl://localhost:7243";  // EMS server URL, default is ssl://localhost:7243"
    string userName = null;                     // user name, default is null
    string password = null;                     // password, default is null
    string topicName = "topic.sample";          // hardcode

    // SSL options
    bool ssl_trace = true;
    ArrayList ssl_trusted = new ArrayList();    // file with trusted certificate(s)
                                                // ../certs/server_root.cert.pem
    string ssl_target_hostname = null;          // CN name expected in the server certificate (server, localhost, localmachine)
    string ssl_identity = null;                 // client identity file {../certs/client_identity.p12}
    string ssl_password = null;                 // password to decrypt client identity or key file {password}


    public tibemsSSL(string[] args)
    {
        ssl_trusted.Add("C:/tibco/ems/8.3/samples/certs/server_root.cert.pem");
        ssl_target_hostname = "localhost";
        ssl_identity = "C:/tibco/ems/8.3/samples/certs/client_identity.p12";
        ssl_password = "password";

        if (topicName == null)
        {
            System.Console.WriteLine("Error: must specify topic name");
            //usage();
        }


        if (topicName == null)
        {
            System.Console.WriteLine("Error: must specify topic name");
        }
        System.Console.WriteLine("SSL sample.");


        // initialize SSL environment
        Hashtable environment = new Hashtable();
        try
        {
            EMSSSLFileStoreInfo storeInfo = new EMSSSLFileStoreInfo();

            // set trace for client-side operations, loading of certificates
            // and other
            if (ssl_trace)
            {
                environment.Add(EMSSSL.TRACE, true);
            }

            // set trusted certificates if specified
            int s = ssl_trusted.Count;
            for (int i = 0; i < s; i++)
            {
                String cert = (String)ssl_trusted[i];
                storeInfo.SetSSLTrustedCertificate(cert);
            }

            // set expected host name in the certificate if specified
            if (ssl_target_hostname != null)
            {
                environment.Add(EMSSSL.TARGET_HOST_NAME, ssl_target_hostname);
            }

            // only pkcs12 or pfx files are supported.
            if (ssl_identity != null)
            {
                if (ssl_password == null)
                {
                    System.Console.WriteLine("Error: must specify -ssl_password if identity is set");
                    System.Environment.Exit(-1);
                }
                storeInfo.SetSSLClientIdentity(ssl_identity);
                storeInfo.SetSSLPassword(ssl_password.ToCharArray());

            }
            environment.Add(EMSSSL.STORE_INFO, storeInfo);
            environment.Add(EMSSSL.STORE_TYPE, EMSSSLStoreType.EMSSSL_STORE_TYPE_FILE);
        }
        catch (Exception e)
        {
            System.Console.WriteLine("ERROR MSG: " + e.Message);
            System.Console.WriteLine(e.StackTrace);

            if (e is EMSException)
            {
                EMSException je = (EMSException)e;
                if (je.LinkedException != null)
                {
                    System.Console.WriteLine("Linked Exception Error Msg: " + e.Message);
                    System.Console.WriteLine("##### Linked Exception:");
                    System.Console.WriteLine(je.LinkedException.StackTrace);
                }
            }
            //System.Environment.Exit(-1);
        }

        try
        {

            ConnectionFactory factory = new ConnectionFactory(serverUrl, null, environment);

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
            System.Console.WriteLine("ERROR MSG: " + e.Message);
            System.Console.WriteLine(e.StackTrace);

            if (e.LinkedException != null)
            {
                System.Console.WriteLine("Linked Exception Error Msg: " + e.Message);
                System.Console.WriteLine("##### Linked Exception:");
                System.Console.WriteLine(e.LinkedException.StackTrace);
            }
            //System.Environment.Exit(-1);
        }
    }

    public static void Main(string[] args)
    {
        tibemsSSL t = new tibemsSSL(args);
        Console.ReadLine();
    }
}

/* 
 * Copyright (c) 2001-2013 TIBCO Software Inc. 
 * All rights reserved.
 * For more information, please contact:
 * TIBCO Software Inc., Palo Alto, California, USA
 * 
 * $Id: emsSSLGlobal.cs 66337 2013-03-28 16:38:23Z $
 * 
 */

/*
 * This is a simple sample of SSL connectivity for
 * EMS .NET clients.  It demonstrates ssl-based client/server communications
 * using global SSL settings.
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
 *  emsSSLGlobal -server ssl://localhost:7243 -ssl_target_hostname <hostname>
 *                  -ssl_identity client_identity.p12 -ssl_password password
 */

using System;
using TIBCO.EMS;
using System.Collections;

/*  References
 *  https://docs.tibco.com/pub/enterprise_message_service/8.1.0/doc/html/tib_ems_api_reference/api/javadoc/com/tibco/tibjms/TibjmsSSL.html
 *  https://tibco4all.com/2016/02/02/how-to-configure-ssl-in-the-ems-server/
 *  http://paulstovell.com/blog/x509certificate2
 *  http://www.ibm.com/support/knowledgecenter/en/SS6QYM_9.1.0/com.ibm.help.secure.deploy.doc/FND_ConfiguringTibco.html
 *  https://www.ca.com/us/services-support/ca-support/ca-support-online/knowledge-base-articles.tec604096.html
 *   
 *  ../certs/readme.txt
 *****************************************************************
 * All PKCS8, PKCS12 and other files encrypted with password:    *
 *                      "password"                               *
 *                                                               *
 *****************************************************************
 *  Issues - Errors
 *  The remote certificate is invalid according to the validation procedure
 *  C:\tibco\ems\8.3\bin>tibemsd.exe -config "C:\ProgramData\TIBCO_HOME\tibco\cfgmgmt\ems\data\tibemsd.conf" -ssl_debug_trace -ssl_trace
 *
 *  See tibemsd.conf in this project for the conf used.
*/

//TIBCO Enterprise Message Service.
//Copyright 2003-2016 by TIBCO Software Inc.
//All rights reserved.

//Version 8.3.0 V14 3/9/2016

//2017-01-12 14:10:47.641 Process started from 'C:\tibco\ems\8.3\bin\tibemsd.exe'.
//2017-01-12 14:10:47.641 Process Id: 3044
//2017-01-12 14:10:47.642 Hostname: DESKTOP-J8LDBJI
//2017-01-12 14:10:47.642 Hostname IP address: [fe80::f085:c3aa:4f63:f896%6]
//2017-01-12 14:10:47.643 Hostname IP address: 192.168.1.48
//2017-01-12 14:10:47.643 Reading configuration from 'C:\ProgramData\TIBCO_HOME\tibco\cfgmgmt\ems\data\tibemsd.conf'.
//2017-01-12 14:10:47.735 Logging into file 'C:/ProgramData/TIBCO_HOME/tibco/cfgmgmt/ems/data/datastore/logfile'
//2017-01-12 14:10:47.737 Server name: 'EMS-SERVER'.
//2017-01-12 14:10:47.737 Storage Location: 'C:/ProgramData/TIBCO_HOME/tibco/cfgmgmt/ems/data/datastore'.
//2017-01-12 14:10:47.737 Routing is disabled.
//2017-01-12 14:10:47.737 Authorization is disabled.
//2017-01-12 14:10:47.742 Secure Socket Layer is enabled, using OpenSSL 1.0.2f-fips  28 Jan 2016
//2017-01-12 14:10:47.743 Accepting connections on tcp://DESKTOP-J8LDBJI/[::]:7222.
//2017-01-12 14:10:47.743 Accepting connections on tcp://DESKTOP-J8LDBJI/0.0.0.0:7222.
//2017-01-12 14:10:47.743 Accepting connections on ssl://DESKTOP-J8LDBJI/[::]:7243.
//2017-01-12 14:10:47.743 Accepting connections on ssl://DESKTOP-J8LDBJI/0.0.0.0:7243.
//2017-01-12 14:10:47.743 Recovering state, please wait.
//2017-01-12 14:10:47.766 Recovered 3 messages.
//2017-01-12 14:10:47.766 Server is active.
//2017-01-12 14:10:53.208 SSL handshake failed: ret=-1, reason=<unknown>
//2017-01-12 14:10:53.208 [OpenSSL Error]: file=ossl.c, line=1699
//2017-01-12 14:10:53.788 SSL handshake failed: ret=-1, reason=<unknown>
//2017-01-12 14:10:53.788 [OpenSSL Error]: file=ossl.c, line=1699
//2017-01-12 14:11:27.961 SSL handshake failed: ret=-1, reason=<unknown>
//2017-01-12 14:11:27.961 [OpenSSL Error]: file=ossl.c, line=1699
//2017-01-12 14:11:28.566 SSL handshake failed: ret=-1, reason=<unknown>
//2017-01-12 14:11:28.566 [OpenSSL Error]: file=ossl.c, line=1699
//2017-01-12 14:12:21.773 Peer certificate:
//2017-01-12 14:12:21.773 Certificate=[/C=US/ST=California/L=us-english/O=Test Company/OU=client Unit/CN=client/emailAddress=client@testcompany.com]
//Issuer=[/C=US/ST=California/L=us-english/O=Test Company/OU=client_root Unit/CN=client_root/emailAddress=client_root@testcompany.com]
//2017-01-12 14:12:21.774 SSL accepted cipher=ECDHE-RSA-AES256-SHA
//2017-01-12 14:12:21.780 [OpenSSL Error]: file=ossl.c, line=2117
//2017-01-12 14:12:22.415 Peer certificate:
//2017-01-12 14:12:22.415 Certificate=[/C=US/ST=California/L=us-english/O=Test Company/OU=client Unit/CN=client/emailAddress=client@testcompany.com]
//Issuer=[/C=US/ST=California/L=us-english/O=Test Company/OU=client_root Unit/CN=client_root/emailAddress=client_root@testcompany.com]
//2017-01-12 14:12:22.415 SSL accepted cipher=ECDHE-RSA-AES256-SHA
//2017-01-12 14:12:22.430 [OpenSSL Error]: file=ossl.c, line=2117


//Global SSL parameters sample.
//2017-01-12 14:12:21.658 [9 ] [TIBCO EMS]: [.NET] [SSL] Creating the SSLStream..
//2017-01-12 14:12:21.664 [9 ] [TIBCO EMS]: [.NET] [SSL] Using store file = C:/tibco/ems/8.3/samples/certs/client_identity.p12
//2017-01-12 14:12:21.708 [9 ] [TIBCO EMS]: [.NET] [SSL] -------------------------
//2017-01-12 14:12:21.708 [9 ] [TIBCO EMS]: [.NET] [SSL] Client Certificate:
//2017-01-12 14:12:21.708 [9 ] [TIBCO EMS]: [.NET] [SSL]            Subject: E= client@testcompany.com, CN= client, OU= client Unit, O= Test Company, L= us - english, S= California, C= US
//2017-01-12 14:12:21.708 [9 ] [TIBCO EMS]: [.NET] [SSL]             Issuer: E= client_root@testcompany.com, CN= client_root, OU= client_root Unit, O= Test Company, L= us - english, S= California, C= US
//2017-01-12 14:12:21.708 [9 ] [TIBCO EMS]: [.NET] [SSL]         Not Before: 2/21/2013 6:28:31 PM
//2017-01-12 14:12:21.708 [9 ] [TIBCO EMS]: [.NET] [SSL]          Not After: 2/19/2023 6:28:31 PM
//2017-01-12 14:12:21.708 [9 ] [TIBCO EMS]: [.NET] [SSL] -------------------------
//2017-01-12 14:12:21.708 [9 ] [TIBCO EMS]: [.NET] [SSL] Authenticating the client
//2017-01-12 14:12:21.777 [9 ] [TIBCO EMS]: [.NET] [SSL] ###########################
//2017-01-12 14:12:21.777 [9 ] [TIBCO EMS]: [.NET] [SSL] Server Certificate:
//2017-01-12 14:12:21.778 [9 ] [TIBCO EMS]: [.NET] [SSL]            Subject: E= server_root@testcompany.com, CN= server_root, OU= server_root Unit, O= Test Company, L= us - english, S= California, C= US
//2017-01-12 14:12:21.778 [9 ] [TIBCO EMS]: [.NET] [SSL]             Issuer: E= server_root@testcompany.com, CN= server_root, OU= server_root Unit, O= Test Company, L= us - english, S= California, C= US
//2017-01-12 14:12:21.778 [9 ] [TIBCO EMS]: [.NET] [SSL]         Not Before: 2/21/2013 6:28:29 PM
//2017-01-12 14:12:21.778 [9 ] [TIBCO EMS]: [.NET] [SSL]          Not After: 2/19/2023 6:28:29 PM
//2017-01-12 14:12:21.778 [9 ] [TIBCO EMS]: [.NET] [SSL] ###########################
//2017-01-12 14:12:22.393 [9 ] [TIBCO EMS]: [.NET] [SSL] Creating the SSLStream..
//2017-01-12 14:12:22.393 [9 ] [TIBCO EMS]: [.NET] [SSL] Using store file = C:/tibco/ems/8.3/samples/certs/client_identity.p12
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL] -------------------------
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL] Client Certificate:
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL]            Subject: E= client@testcompany.com, CN= client, OU= client Unit, O= Test Company, L= us - english, S= California, C= US
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL]             Issuer: E= client_root@testcompany.com, CN= client_root, OU= client_root Unit, O= Test Company, L= us - english, S= California, C= US
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL]         Not Before: 2/21/2013 6:28:31 PM
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL]          Not After: 2/19/2023 6:28:31 PM
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL] -------------------------
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL] Authenticating the client
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL] ###########################
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL] Server Certificate:
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL]            Subject: E= server_root@testcompany.com, CN= server_root, OU= server_root Unit, O= Test Company, L= us - english, S= California, C= US
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL]             Issuer: E= server_root@testcompany.com, CN= server_root, OU= server_root Unit, O= Test Company, L= us - english, S= California, C= US
//2017-01-12 14:12:22.415 [9 ] [TIBCO EMS]: [.NET] [SSL]         Not Before: 2/21/2013 6:28:29 PM
//2017-01-12 14:12:22.430 [9 ] [TIBCO EMS]: [.NET] [SSL]          Not After: 2/19/2023 6:28:29 PM
//2017-01-12 14:12:22.430 [9 ] [TIBCO EMS]: [.NET] [SSL] ###########################
//##### Exception:Failed to connect via SSL to [ssl://localhost:7243]: Failed to connect via SSL to [ssl://localhost:7243]: The remote certificate is invalid according to the validation procedure.
//   at TIBCO.EMS.CFImpl._CreateConnection(String userName, String password, Boolean xa)
//   at TIBCO.EMS.ConnectionFactory.CreateConnection(String userName, String password)
//   at emsSSLGlobal..ctor(String[] args) in C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\tibcoems-tutorials\TibcoEMS-tutorials\6_emsSSLSampleClient\Program.cs:line 123
//##### Linked Exception error msg:Failed to connect via SSL to [ssl://localhost:7243]: The remote certificate is invalid according to the validation procedure.
//##### Linked Exception:
//   at TIBCO.EMS.LinkSSL.Connect(URL url)

public class emsSSLGlobal
{
    string serverUrl = "ssl://localhost:7243";
    string userName = null;
    string password = null;
    string topicName = "topic.sample";

    // SSL options
    bool ssl_trace = true;
    ArrayList ssl_trusted = new ArrayList();
    string ssl_target_hostname = "SampleAuthority";
    string ssl_identity = "C:/tibco/ems/8.3/samples/certs/client_identity.p12";
    string ssl_password = "password";

    public emsSSLGlobal(string[] args)
    {
        //parseArgs(args);

        if (topicName == null)
        {
            System.Console.WriteLine("Error: must specify topic name");
            //usage();
        }

        System.Console.WriteLine("Global SSL parameters sample.");

        try
        {
            EMSSSLFileStoreInfo storeInfo = new EMSSSLFileStoreInfo();

            // set trace for client-side operations, loading of certificates
            // and other
            if (ssl_trace)
            {
                EMSSSL.SetClientTracer(new System.IO.StreamWriter(System.Console.OpenStandardError()));
            }


            // set trusted certificates if specified
            int s = ssl_trusted.Count;
            for (int i = 0; i < s; i++)
            {
                string cert = (string)ssl_trusted[i];
                storeInfo.SetSSLTrustedCertificate(cert);
            }

            // set target host name in the sertificate if specified
            if (ssl_target_hostname != null)
            {
                EMSSSL.SetTargetHostName(ssl_target_hostname);
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

            EMSSSL.SetCertificateStoreType(EMSSSLStoreType.EMSSSL_STORE_TYPE_FILE, storeInfo);
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
            System.Console.WriteLine("##### Exception:" + e.Message);
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
        emsSSLGlobal t = new emsSSLGlobal(args);
        Console.ReadLine();
    }
}
/* 
 * Copyright (c) 2001-2013 TIBCO Software Inc. 
 * All rights reserved.
 * For more information, please contact:
 * TIBCO Software Inc., Palo Alto, California, USA
 * 
 * $Id: emsSSLSampleClient.cs 66337 2013-03-28 16:38:23Z $
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
 * NOTE: The makecert tool from .NET 2.0 framework is required.
 *
 * Example usage:
 * creating a self signed certificate
 *    This creates a SampleAuthority and added it to the currentuser store location
 *       makecert -pe -n "CN=SampleAuthority" -ss My -sr CurrentUser -a sha1 -sky Signature -r "SampleAuthority.cer" 
 *    This creates a certificate signed by the Sample Authority
 *       makecert -pe -ss my -sr CurrentUser -a sha1 -sky exchange -in "SampleAuthority" -is My -ir CurrentUser -sp "Microsoft RSA Schannel Cryptographic Provider" -sy 12 -n "CN=client-sample-cert" -b 01/01/2016 -e 01/01/2999 -eku 1.3.6.1.5.5.7.3.2 Client.cer
 * 
 * You will need to convert the DER encoded file to base64 encoded file to add
 * it to the client_root.cert.pem file of the EMS server's certificate store.
 * One way to achieve this is to.
 * Open the Microsoft management console and import the SampleAuthority certificate into
 * a base64 encoded file and open this file and copy everything betweeen
 * -----BEGIN CERTIFICATE----- and -----END CERTIFICATE----- into the EMS server's
 * client_root.cert.pem file. 
 *
 *  What do I do with the client_root.cert.pem file???? 
 *  How is tibemsd.conf setup?
 *  
 *  
 *  emsSSLSampleClient -server ssl://localhost:7243 -ssl_target_hostname <hostname>
 *                  -ssl_cert_store_location currentuser -ssl_cert_store_name My -ssl_cert_name "CN=client-sample-cert"
 */

using System;
using TIBCO.EMS;
using System.Security.Cryptography.X509Certificates;
using System.Net;


//Global SSL parameters sample with Microsoft Cerrtificate Store.
//##### Exception:Failed to connect via SSL to [ssl://localhost:7243]: Failed to connect via SSL to [ssl://localhost:7243]: The remote certificate is invalid according to the validation procedure.
//   at TIBCO.EMS.CFImpl._CreateConnection(String userName, String password, Boolean xa)
//   at TIBCO.EMS.ConnectionFactory.CreateConnection(String userName, String password)
//   at emsSSLSampleClient..ctor(String[] args) in C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\tibcoems-tutorials\TibcoEMS-tutorials\6_emsSSLSampleClient2\Program.cs:line 147
//##### Linked Exception error msg:Failed to connect via SSL to [ssl://localhost:7243]: 
// The remote certificate is invalid according to the validation procedure.
//##### Linked Exception:
//   at TIBCO.EMS.LinkSSL.Connect(URL url)
// Error: The remote certificate is invalid according to the validation procedure.

// Put both certs in the Trusted Root Certifications Authority...
// Note, they are both missing private keys... is this the issue?
// I found the private keys.... see references...
// Global SSL parameters sample with Microsoft Cerrtificate Store.
//##### Exception:Failed to connect via SSL to [ssl://localhost:7243]: Failed to connect via SSL to [ssl://localhost:7243]: The credentials supplied to the package were not recognized
//   at TIBCO.EMS.CFImpl._CreateConnection(String userName, String password, Boolean xa)
//   at TIBCO.EMS.ConnectionFactory.CreateConnection(String userName, String password)
//   at emsSSLSampleClient..ctor(String[] args) in C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\tibcoems-tutorials\TibcoEMS-tutorials\6_emsSSLSampleClient2\Program.cs:line 158
//##### Linked Exception error msg:Failed to connect via SSL to [ssl://localhost:7243]: The credentials supplied to the package were not recognized
//##### Linked Exception:
//   at TIBCO.EMS.LinkSSL.Connect(URL url)
// Error: The credentials supplied to the package were not recognized

//2017-01-12 20:32:02.852 Server is active.
//2017-01-12 20:32:11.489 Peer has no certificate
//2017-01-12 20:32:11.489 SSL accepted cipher=ECDHE-RSA-AES256-SHA
//2017-01-12 20:32:11.508 [OpenSSL Error]: file=ossl.c, line=2117
//2017-01-12 20:32:12.040 Peer has no certificate
//2017-01-12 20:32:12.040 SSL accepted cipher=ECDHE-RSA-AES256-SHA
//2017-01-12 20:32:12.040 [OpenSSL Error]: file=ossl.c, line=2117


/*
 *   Refernces:
 *   http://paulstovell.com/blog/x509certificate2
 *   http://www.ibm.com/support/knowledgecenter/en/SS6QYM_9.1.0/com.ibm.help.secure.deploy.doc/FND_ConfiguringTibco.html
 *   https://www.ca.com/us/services-support/ca-support/ca-support-online/knowledge-base-articles.tec604096.html
 *   
 *   C:\Users\Craig Nicholson\AppData\Roaming\Microsoft\SystemCertificates\My\Certificates
 *   C:\Users\Craig Nicholson\AppData\Roaming\Microsoft\SystemCertificates\My\Keys\
 *   C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys\
 *   C:\Users\Paul\AppData\Roaming\Microsoft\Crypto\RSA\
 */


public class emsSSLSampleClient
{
    string serverUrl = "ssl://localhost:7243";
    string userName = null;
    string password = null;
    string topicName = "topic.sample";

    // SSL options - certs are pulled from machines certificate store... but how does tibco ems know what to expect?  Does it need
    // a reference to SampleAuthority?
    bool ssl_trace = false;
    string ssl_target_hostname = "SampleAuthority";
    string ssl_cert_store_location = "localmachine";  //localmachine or currentuser
    string ssl_cert_store_name = "My";
    string ssl_cert_name = "CN=client-sample-cert";

    public emsSSLSampleClient(string[] args)
    {
        if (topicName == null)
        {
            System.Console.WriteLine("Error: must specify topic name");
        }

        System.Console.WriteLine("Global SSL parameters sample with Microsoft Cerrtificate Store.");
        try
        {
            // System Store Info object to be used while setting the store type for a connection factory via the 
            // ConnectionFactory.SetCertificateStoreType. The store info consists of the store location, store name, 
            // the certificate name (to look for in the specified store name at the specified store location).
            // The default store location is StoreLocation.CurrentUser and the default store name is 'my' store as defined by the .NET framework. 
            // The search criteria to find the certificate in the store name at the store location is X509FindType.FindBySubjectDistinguishedName.
            EMSSSLSystemStoreInfo storeInfo = new EMSSSLSystemStoreInfo();
           
            // set trace for client-side operations, loading of certificates
            // and other
            if (ssl_trace)
            {
                EMSSSL.SetClientTracer(new System.IO.StreamWriter(System.Console.OpenStandardError()));
            }

            // Set the target host name.
            // This is a required parameter for all.NET SSL connections.Because System.Net.Security.SslStream 
            // requires a target host, this value is required.
            //
            // The name of the server as defined in the server's certificate. Usually the server's HostName 
            // is specified as the CN in the server's certificate. This value must match the name on the
            // server's certificate server name.
            if (ssl_target_hostname != null)
            {
                EMSSSL.SetTargetHostName(ssl_target_hostname);
            }

            // Set location of the certificate store
            // The certificate store location indicates where to lookup the certificate by name. If no store name is specified, 
            // then the default store name is "My" store name within this store location.
            // storeLocation	Location in which to lookup certificate by name. For example, "CurrentUser" or "LocalMachine."
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

            // Set the certificate store name
            // This is the name of the store in which certificates are stored. During the SSL handshake, 
            // this is where the client library looks for the certificates.
            if (ssl_cert_store_name != null)
            {
                storeInfo.SetCertificateStoreName(ssl_cert_store_name);
            }

            // Set the name of the certificate as a full subject DN
            // This method sets the name of the certificate. The certificate name is the subject distinguished name of the certificate. 
            // During the SSL handshake, the server searches for the named certificate in the store specified by SetCertificateStoreName 
            // at the location specified by SetCertificateStoreLocation. The search criteria to find the certificate in the store name 
            // at the store location is X509FindType.FindBySubjectDistinguishedName. 
            // A subject DN sample
            //      E=client@testcompany.com, CN=client, OU=client Unit, O=Test Company, L=us-english, S=California, C=US
            if (ssl_cert_name != null)
            {
                storeInfo.SetCertificateNameAsFullSubjectDN(ssl_cert_name);
            }

            // Set the store type for ALL the connection factories
            // If the store type is EMSSSL_STORE_TYPE_SYSTEM, then storeInfo must be an EMSSSLSystemStoreInfo object. If the store type is 
            // EMSSSL_STORE_TYPE_FILE, then storeInfo must be an EMSSSLFileStoreInfo object.
            // The type of certificate store. Can be either EMSSSL_STORE_TYPE_SYSTEM or EMSSSL_STORE_TYPE_FILE. See EMSSSLStoreType details.
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
        }

        try
        {
            ConnectionFactory factory = new ConnectionFactory(serverUrl);

            //How can I wrap this to ignore self signed certs mistrust
            // this does not work for our issues....
            ServicePointManager.ServerCertificateValidationCallback = delegate (
                     object obj, X509Certificate certificate, X509Chain chain,
                     System.Net.Security.SslPolicyErrors errors)
            {
                return (true);
            };

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
        }
    }

    public static void Main(string[] args)
    {
        emsSSLSampleClient t = new emsSSLSampleClient(args);
        Console.ReadLine();
    }
}
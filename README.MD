# DotNet C# code for Tibco EMS Tutorials 
Here you can find C# examples for Tibco EMS.  Examples where created in VS2015 on Windows 10.

To successfully use the examples you will need a running Tibco EMS server.

## Requirements
This is a work in progress as I learn Tibco EMS.

### Requirements for Windows
* dotnet 4.5 VS2015
* Tibco.EMS.dll vs v4.0.30319, for Tibco EMS 8.3
* Tibco EMS Server running

## Tibco EMS Install - Windows
Use the installer and accept all the default settings (TIB_ems-dev_8.3.0_win_x86_64_vc10.zip)  

Start the EMS Server from Windows Menu  
* TIBCO > Start EMS Server  

This will open a windows command prompt and the server will be running.  

You can skip setting the EMS Server as a windows sevice and just start it manually for local development or running these samples.  

Start the EMS Administration Tool from Windows Menu  
* TIBCO > Start EMS Administration Tool   

After starting you will need to connect to the running EMS Server.  The admin and passwords will be blank, just hit the enter key.   
Here are a few commands you can run in the a cmd window while the EMS Server is running.   

> connect  
  Login name (admin):    
  Password:    
  Connected to: tcp://localhost:7222    
  tcp://localhost:7222>    

The main commands we are interested in are below.  While you run these examples you can use 'show topics and 'show queues' to
see the messages are added on the topic or queue.  

> help  
  show topics  
  show topic topicname  
  show queues  
  show queue queuename  
  delete topic topicname  
  delete queue queuename  

[Additional Commands](https://docs.tibco.com/pub/ems/8.3.0/doc/html/wwhelp/wwhimpl/js/html/wwhelp.htm)

## Code Samples

#### Tutorial 0: "Hello World"

- [0_ReceiveHelloWorld](https://github.com/craignicholson/tibcoems-tutorials/tree/master/0_ReceiveHelloWorld)  
- [0_SendHelloWorld](https://github.com/craignicholson/tibcoems-tutorials/tree/master/0_SendHelloWorld)  

You can either start ReceiveHelloWorld first or SendHelloWorld.  This example uses a Point-to-Point queue
where the producers messages are saved in the queue until a consumer removes the messages and acknowledges receipt.

##### Putting It All Together
Run the executable

> $ 1_Receive.exe

then, run the sender:

> $ 1_Send.exe

The receiver will print the message it received from the Tibco EMS. 

If you want to review the message on the queue, try using EMS Administrator Server show queues.

#### Tutorial 1: Publish/Subscribe

- [1_Receive](https://github.com/craignicholson/tibcoems-tutorials/tree/master/1_Receive)  
- [1_Send](https://github.com/craignicholson/tibcoems-tutorials/tree/master/1_Send)  

Start two or more instances of Receiver (subscriber/consumer) and then start the Send publilsher (producer).  

A Hello World message will be published to all subscribes which are online.  If a subscriber is offline
it will not receive the message.  To have an offline subscriber still receive the message we will need
to create a durable subscriber. 

- [1_ReceiveDurable](https://github.com/craignicholson/tibcoems-tutorials/tree/master/1_ReceiveDurable)

By default, subscribers only receive messages when they are active. If messages arrive on the topic when the subscriber is not available, the subscriber does not receive those messages.
The EMS APIs allow you to create durable subscribers to ensure that messages are received, even if the message consumer is not currently running. Messages for durable subscriptions are stored on the server as long as durable subscribers exist for the topic, or until the message expiration time for the message has been reached, or until the storage limit has been reached for the topic. Durable subscribers can receive messages from a durable subscription even if the subscriber was not available when the message was originally delivered.
When an application restarts and recreates a durable subscriber with the same ID, all messages stored on the server for that topic are delivered to the durable subscriber.
See Creating a Message Consumer for details on how to create durable subscribers.

Once 1_ReceiveDurable has ran once and created a durable subriber to the topic if this client stops and messages
are delivered to the topic, once the client is restarted the client will receive the messages.

> tcp://localhost:7222> show topic GeneralTopic

```
 
 Topic:                 GeneralTopic
 Type:                  dynamic
 Properties:            *prefetch=64,*store=$sys.nonfailsafe
 JNDI Names:            <none>
 Bridges:               <none>
 Subscriptions:         1
 Durable Subscriptions: 1
 Consumers:             0
 Durable Consumers:     0
 Pending Msgs:          0, (0 persistent)
 Pending Msgs Size:     0.0 Kb, (0.0 Kb persistent)
 Total Inbound Msgs:    6
 Total Inbound Bytes:   0.8 Kb
 Total Outbound Msgs:   11
 Total Outbound Bytes:  1.5 Kb

 ```

> tcp://localhost:7222> show durables

```
  Topic Name    Durable                                Shared  User         Msgs    Size
  GeneralTopic  Craig.Nicholson:helloworld.subscriber  N       <offline>       0     0.0 Kb

```

#### Tutorial 2: Send Files to a Queue

- [2_ReceiveByteMessage](https://github.com/craignicholson/tibcoems-tutorials/tree/master/2_ReceiveByteMessage)  
- [2_SendByteMessage](https://github.com/craignicholson/tibcoems-tutorials/tree/master/2_SendByteMessage)  

Either ReceiveByteMessage or SendByteMessage can be started.  The example uses a Point-to-Point queue
which allows for one message per consumer.

This use case is an example of where data needs to be exported out of a system and delivered to another 
vendor.  Instead of writing a file to disk, pushing the file to a ftp site, giving the ftp credentials
to the vendor, and the vendor pulling off the files as they are published, we just write a byte message
to a queue and the consumer receives the messages from the queue.  

This example uses an xml file which is then written to the queue as a byte array.  We read the bytes from
the file and write the bytes to a ByteMessage.  The Consumer then receives the message and reads the bytes
and writes the data back to disk as an xml file.

#### Tutorial 3: Send XML as TextMessage

- [3_ReceiveTextMessage](https://github.com/craignicholson/tibcoems-tutorials/tree/master/3_ReceiveTextMessage)  
- [3_SendTextMessage](https://github.com/craignicholson/tibcoems-tutorials/tree/master/3_SendTextMessage)  

This use case is an example of one way to send detailed messages to Workers or even using Publisher and Subscibers.
Another use case is if we are receiving soap messages which we need to re-publish to many subscribers.

#### Tutorial 4: Work Queues

- [4_Worker](https://github.com/craignicholson/tibcoems-tutorials/tree/master/4_Worker)  
- [4_Tasks](https://github.com/craignicholson/tibcoems-tutorials/tree/master/4_Tasks)  

One of the benefits of the Point-to-Point queue is you can keep adding messages to the queue and add more
consumers as the work load increases.

Start one or more Workers (Consumers) and then start the Tasks (Producer).   The Tasks app will generate
1000 numbers and each Worker will read one message and perform prime factorization on the number.

You can also add more Workers while the Tasks and other Workers are already running to help process messages.

#### Tutorial 5: Routing
IS this doable with TIBCO?
- 4_ReceiveLogsDirect
- 4_EmitLogDirect

#### Tutorial 6: Topics

- 5_ReceiveLogsTopic
- 5_EmitLogTopic

#### Tutorial 7: RPC

- 6_RPCServer
- 6_RPCClient

#### Tutorial 7:# Asynchrounous Message Consumer

#### Tutorial 8: QueueBrowser

#### Tutorial 9: MultiCast Example

#### Tutorial 10 SSL Example
Starting EMS with SSL
1. EMS .NET 2.0 clients use the Microsoft implementation of SSL. The Microsoft implementation of SSL is compatible with OpenSSL. Certificates required by the client can either be stored in files or the Microsoft certificate store. However, Microsoft requires that the root certificate be installed in the Microsoft Certificate Store, even when certificate files are in use.

https://docs.tibco.com/pub/enterprise_message_service/8.1.0/doc/html/tib_ems_users_guide/wwhelp/wwhimpl/js/html/wwhelp.htm#href=EMS.5.084.htm#4999850

Location of 
C:\tibco\ems\8.3\bin
tibemsd.conf

Edit tibemsd.conf
https://docs.tibco.com/pub/enterprise_message_service/8.1.0/doc/html/tib_ems_users_guide/wwhelp/wwhimpl/js/html/wwhelp.htm#href=EMS.5.084.htm#4999850

Set the server to listen for SSL connections from clients by using the listen parameter in tibemsd.conf. To specify that a port accept SSL connections, specify the SSL protocol in the listen parameter as follows:

```

TIBCO Enterprise Message Service.
Copyright 2003-2016 by TIBCO Software Inc.
All rights reserved.

Version 8.3.0 V14 3/9/2016

2017-01-10 14:25:25.542 Process started from 'C:\tibco\ems\8.3\bin\tibemsd.exe'.
2017-01-10 14:25:25.542 Process Id: 16548
2017-01-10 14:25:25.542 Hostname: DESKTOP-J8LDBJI
2017-01-10 14:25:25.542 Hostname IP address: [fe80::f085:c3aa:4f63:f896%5]
2017-01-10 14:25:25.542 Hostname IP address: 192.168.1.48
2017-01-10 14:25:25.542 Reading configuration from 'C:\ProgramData\TIBCO_HOME\tibco\cfgmgmt/ems/data/tibemsd.conf'.
2017-01-10 14:25:25.627 Logging into file 'C:/ProgramData/TIBCO_HOME/tibco/cfgmgmt/ems/data/datastore/logfile'
2017-01-10 14:25:25.627 Server name: 'EMS-SERVER'.
2017-01-10 14:25:25.627 Storage Location: 'C:/ProgramData/TIBCO_HOME/tibco/cfgmgmt/ems/data/datastore'.
2017-01-10 14:25:25.627 Routing is disabled.
2017-01-10 14:25:25.627 Authorization is disabled.
2017-01-10 14:25:25.627 Accepting connections on tcp://DESKTOP-J8LDBJI/[::]:7222.
2017-01-10 14:25:25.642 Accepting connections on tcp://DESKTOP-J8LDBJI/0.0.0.0:7222.
2017-01-10 14:25:25.642 Recovering state, please wait.
2017-01-10 14:25:25.658 Recovered 3 messages.
2017-01-10 14:25:25.658 Server is active.

```
C:\ProgramData\TIBCO_HOME\tibco\cfgmgmt/ems/data/tibemsd.conf
 
```txt

 ########################################################################
# Listen ports. May be tcp or ssl, can specify any number.
# Form is tcp://hostname:port. If the hostname is not present then 
# the default host and interface will be used.
########################################################################

#listen                 = tcp://7222
listen					= ssl://localhost:7243

########################################################################
# SSL Server Setup Information.
#
# These parameters define server-side certificate, private key, issuers
# of client certificates and miscellaneous parameters used by this EMS
# server when handling SSL connections from the clients and other EMS
# servers.
########################################################################

# specify Diffie-Hellman key size, valid values are 512, 768, 1024, 2048.
# Default is 1024. Not used for export grade cipher suites.

ssl_dh_size             =

# can be used to disable specific ciphers or change the 
# priority order. This is used in the format accepted
# by OpenSSL, refer to OpenSSL documentation for more info.
# Example: ssl_ciphers = +DES-CBC3-SHA:+RC4-SHA

ssl_server_ciphers      =

# The following is set if all clients must preset the certificate.
# If disabled the client may or may not have the certificate.

ssl_require_client_cert =

# This enforces the policy when the connection username is always
# extracted from the certificate, if the certificate was presented
# by the client. This does not affect users who have no certificate.

ssl_use_cert_username   =

# This specifies a special username, when presented as connection user
# name, the actual username is extracted from the client certificate
# if client has the certificate. This does not affect users who have
# no certificate.

ssl_cert_user_specname  = CERTIFICATE_USER

# Server certificate, key and private key password. If password not
# specified it is prompted for at start up time. The key and server
# certificate issuers may be included into specified PKCS12 file.
# Supports PEM, DER and PKCS12.

ssl_server_identity     = "C:/tibco/ems/8.3/certs/server.cert.pem"
ssl_server_key          = "C:/tibco/ems/8.3/certs/server.key.pem"
ssl_password            = $man$WjtSRCpaXu7hoTkDlcEPr6KNKRr

# Server Issuer certificate(s).
# Supports PEM, DER and PKCS#12.
# This may be a part of PKCS12 specified by ssl_server_identity

ssl_server_issuer       = 

# Trusted issuers of client certificates. Supports PEM, DER and PKCS7.

ssl_server_trusted      = "C:/tibco/ems/8.3/certs/client_root.cert.pem"

# Path to installed Enthropy Gathering Daemon

ssl_rand_egd            = 
```


Windows 10 - MakeCert
https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk

C:\Program Files (x86)\Windows Kits\10\

Create the Certificate

MMC to convert ot base64
 * Open the Microsoft management console and import the SampleAuthority certificate into a Certificate Store.
 * I used Trusted Root Certification Authorities
 * Double Click into Trusted Root Certification Authorities
 * Open with dbl click the SampleAuthority
 * Click Details Tab
 * Click Copy to File
 * Click Next
 * Select Base-64 encoded X.509 (.cer). Save to your project as SampleAuthorityBase64.cer.
 * Open this file and copy everything betweeen
  -----BEGIN CERTIFICATE----- and -----END CERTIFICATE----- into the EMS server's
  client_root.cert.pem file. 
* what next...

## Message Models
[Short review of Tibco EMS documentation](https://docs.tibco.com/pub/enterprise_message_service/8.1.0/doc/html/tib_ems_users_guide/wwhelp/wwhimpl/js/html/wwhelp.htm#href=EMS.5.084.htm#4999850)

### Point-to-Point
One producer and one consumer per message or multiple producures and consumers on one queue.

#### Publish and Subscribe
Producers address messages to a topic.
Durable Subscribers for topics.
Shared Subscriptions for topics.

#### MultiCast
Allows one producure to send a message to multiple subscribers simultaneously.
Read up: does not send a copy to each.  ... using EMS Multicast Daemon... on each client.
Like EMS sends signal to each client's EMD Multicast Daemon, and the client pulls the message????
Reduces bandwidth...

## Message Delivery Modes
- PERSISTENT
Producer waits for client to reply with confirmation.  Writes data to disk.   Data remains until client sends ACK.

- NON_PERSISTENT
Data in memory, if no client or EMS restarts data is lost.

- RELAIABLE_DELIVERY - ...

#### Queues
If a consumer is connected the message will be dispatched to it. If there are no consumers connected the message will remain saved on disk until a consumer connects, whereupon it will be dispatched.

When a non-persistent message is sent to a queue, the broker saves it in memory. When a consumer connects the message is dispatched to it. If the broker is restarted before the message is sent then it will be lost forever.

#### Topics

## Message Header

```
Published message: 
BytesMessage=
    { 
        Header={ 
            MessageID={ID:EMS-SERVER.1BE45852C34B3:1} 
            Destination={Queue[ExportQ]} 
            ReplyTo={} 
            DeliveryMode={Persistent} 
            Redelivered={False} 
            CorrelationID={} 
            MsgType={Byte} 
            Timestamp={12/15/2016 4:23:11 PM} 
            DeliveryTime={12/15/2016 4:23:11 PM} 
            Expiration={0} 
            Priority={4} 
        } 
        Properties={ 
            FILE_NAME={String:5763a35e-8271-4ec6-986d-0340951ee40c} 
            FILE_SIZE={String:1259119} 
        } 
        Bytes={1259119 bytes} 
    }

```

#### Message Types
The examples will use TextMessage and BytesMessage because these are the two types we work with when this tutorial was created.
* Message
* TextMessage
* MapMessage
* BytesMessage
* StreamMessage
* ObjectMessage

Maximum message size is 512MB.

# References
https://docs.tibco.com/pub/ems/8.3.0/doc/html/wwhelp/wwhimpl/js/html/wwhelp.
http://fahdshariff.blogspot.com/2010/08/using-compressed-jms-messages.html


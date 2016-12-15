# DotNet C# code for Tibco EMS Tutorials 
Here you can find C# examples for Tibco EMS.

To successfully use the examples you will need a running Tibco EMS server.

## Requirements

### Requirements for Windows
* dotnet 4.5 VS2015
* Tibco.EMS.dll vs v4.0.30319, for Tibco EMS 8.3
* Tibco EMS Server running

## Tibco EMS Install - Windows
Use the installer and accept all the default settings
Manually Starting... Menu TIBCO > Start EMS Server
This will open a windows command prompt and the server will be running...

You can skip setting the EMS Server as a windows sevice and just start it manually for local development.

### Tibco EMS Commands
Here are a few commands you can run in the a cmd window while the EMS Server is running.
Manually Starting... Menu TIBCO > Start EMS Administration Tool

> connect
Login name (admin):
Password:
Connected to: tcp://localhost:7222
tcp://localhost:7222>

Admin and password should be both blank, this is a local dev machine.  production and dev integration testing should have admin setup.
The main commands we are interested in are below.  

> help
> show topics
> show topic topicname
> show queues
> show queue queuename
> delete topic topicname
> delete queue queuename

[More Commands](https://haritibcoblog.com/2015/05/23/tibco-ems-administration-commands-some-important-ones/)

## Message Models

### Point-to-Point
One producure and one consumer per message or multiple producures and consumers on one queue.
Are all queues of this type at SSN?

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
PERSISTENT- producer waits for client to reply with confirmation.  Writes data to disk.   Data remains until client sends ACK.
NON_PERSISTENT - data in memory, if no client or EMS restarts data is lost.
RELAIABLE_DELIVERY - ...

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

## Code

#### [Tutorial 1: "Hello World!" using TextMessage and Topic]()

#### [Tutorial 2: Push a file to the queue, and receive the bytes and write the file back out using ByteMessage and Queue]()

#### [Tutorial 3: Sending XML using TextMessage and Topic]()

#### [Pub Sub Example]()

#### [MultiCast Example]()

#### [Durable Subscriber]()

#### [SSL Example]()

#### [Examples from Tibco]()

# References
https://docs.tibco.com/pub/ems/8.3.0/doc/html/wwhelp/wwhimpl/js/html/wwhelp.htm

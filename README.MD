# DotNet C# code for Tibco EMS Tutorials 
Here you can find C# examples for Tibco EMS.

To successfully use the examples you will need a running Tibco EMS server.

## Requirements

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

### Tibco EMS Commands
Here are a few commands you can run in the a cmd window while the EMS Server is running.  

Start the EMS Administration Tool from Windows Menu  
* TIBCO > Start EMS Administration Tool   

After starting you will need to connect to the running EMS Server.  The admin and passwords will be blank, just hit the enter key.   

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

[Additional Commands](https://haritibcoblog.com/2015/05/23/tibco-ems-administration-commands-some-important-ones/)

## Code Samples

#### Tutorial 1: "Hello World!" using TextMessage and Topic
In a publish and subscribe message system, producers address messages to a topic. In this model, the producer 
is known as a publisher and the consumer is known as a subscriber.  

Many publishers can publish to the same topic, and a message from a single publisher can be received by many 
subscribers. Subscribers subscribe to topics, and all messages published to the topic are received by all 
subscribers to the topic. This type of message protocol is also known as broadcast messaging because messages 
are sent over the network and received by all interested subscribers, similar to how radio or 
television signals are broadcast and received.   (need to write my own description)  

[Consumer-Subscriber](https://github.com/craignicholson/tibcoems-tutorials/tree/master/1_Receive)   
[Producer-Publisher](https://github.com/craignicholson/tibcoems-tutorials/tree/master/1_Send)  


Start the Consumer first (You can start one or more than one)  
Start the Producer - which sends the Hello World message to all subscribers.  

explain the message selector...  
explain the async message handler in 1_Receive  

#### Tutorial 2: Push a file to the queue, and receive the bytes and write the file back out using ByteMessage and Queue
Point-to-point messaging has one producer and one consumer per message. 
This style of messaging uses a queue to store messages until they are received. 
The message producer sends the message to the queue; the message consumer retrieves 
messages from the queue and sends acknowledgement that the message was received.

[Consumer](https://github.com/craignicholson/tibcoems-tutorials/tree/master/2_ReceiveByteMessage)  
[Producer](https://github.com/craignicholson/tibcoems-tutorials/tree/master/2_SendByteMessage)  


You can run 2_SendByteMessage before you run 2_ReviceByteMessage  
You can run 2_ReceiveByteMessagae before you run 2_SendByteMessage  

The Queue will persist the message until you remove the messages off of the queue.  

Only one recevier works for this example.   Multiple receivers will not recieve the message.  

I wonder is I send 100000's of requests if multiple receivers will pull other messages off the queue?   


#### Tutorial 3: Sending XML using TextMessage and Queue
Point-to-Point Use Case

Applications are processing information and when the information is ready the data is posted to the queue.
The Consumer receives the data and loads it into a database.

[Consumer](https://github.com/craignicholson/tibcoems-tutorials/tree/master/2_ReceiveTextMessage)   
[Producer](https://github.com/craignicholson/tibcoems-tutorials/tree/master/2_SendTextMessage)  

#### Asynchrounous Message Consumer

#### QueueBrowser

#### Pub Sub Example

#### MultiCast Example

#### Durable Subscriber

#### SSL Example

#### Examples from Tibco

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


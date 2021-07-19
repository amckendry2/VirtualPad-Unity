using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.Sockets;

namespace VirtualPadUtils

{

public class UDPClient
{
    private UdpClient client = new UdpClient();
    private BlockingCollection<byte[]> sendQueue { get; }
    public ConcurrentQueue<byte[]> receiveQueue { get; }
    public Thread receiveThread {get; set;}
    public Thread sendThread {get; set;}
    private string serverURL;
    private int serverPort;

    // public event EventHandler GotData; 

    public UDPClient(int port, string _serverURL, int _serverPort) {
        client = new UdpClient(port);
        serverURL = _serverURL;
        serverPort = _serverPort;
        receiveQueue = new ConcurrentQueue<byte[]>();
        sendQueue = new BlockingCollection<byte[]>();
        receiveThread = new Thread(RunReceive);
        receiveThread.Start();
        sendThread = new Thread(RunSend);
        sendThread.Start();    
    } 

    private async void RunSend() {
        byte[] msg;
        while (true) {
            while (!sendQueue.IsCompleted){
                msg = sendQueue.Take();
                await client.SendAsync(msg, msg.Length, serverURL, serverPort);
            }
        }
    }

    public void SendRequest(byte[] msg){
        sendQueue.Add(msg);  
    }

    private async void RunReceive() {
        while (true) {
            UdpReceiveResult receiveResult = await client.ReceiveAsync();
            byte[] bufData = receiveResult.Buffer;
            receiveQueue.Enqueue(bufData);
        }
    }

    public void Close() {
        client.Close();
    }
}

}



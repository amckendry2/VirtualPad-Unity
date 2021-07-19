using System.Text;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using VirtualPadUtils.CommUtils;

namespace VirtualPadUtils

{

public class UDPHandler {

    private Dictionary<UDPEventType, EventHandler> EventDictionary;

    private event EventHandler ConnectEvent;
    private event EventHandler ConnectionFailedEvent;
    private event EventHandler DisconnectEvent;
    private event EventHandler InputEvent;
    private event EventHandler PingEvent;
    private event EventHandler PlayerJoinedEvent;
    private event EventHandler PlayerLeftEvent;

    private UTF8Encoding encoding;
    private UDPClient client;
    private InputState[] PlayerInputs;

    private Thread pingSendThread;
    private Thread pingReceiveThread;
    
    public string gameCode;
    public bool connectedToServer;
    public int maxPlayers;
    public int currentPlayers;

    private bool pinged;
    private int localPort;
    private string serverURL;
    private int serverPort;

    
    public UDPHandler(int localPort, string serverURL, int serverPort, int maxPlayers){
        encoding = new UTF8Encoding();
        PlayerInputs = new InputState[maxPlayers];
        for(int i=0; i< maxPlayers; i++){
            PlayerInputs[i] = new InputState();
        }
        this.localPort = localPort;
        this.serverURL = serverURL;
        this.serverPort = serverPort;  
        this.maxPlayers = maxPlayers;
        this.currentPlayers = 0;

        EventDictionary = new Dictionary<UDPEventType, EventHandler>(){
            {UDPEventType.INPUT, InputEvent},
            {UDPEventType.CONNECTION, ConnectEvent},
            {UDPEventType.CONNECTION_FAILED, ConnectionFailedEvent},
            {UDPEventType.DISCONNECT, DisconnectEvent},
            {UDPEventType.PLAYER_JOINED, PlayerJoinedEvent},
            {UDPEventType.PLAYER_LEFT, PlayerLeftEvent},
            {UDPEventType.PING, PingEvent}
        };
    }

    public void RegisterGameWithServer(){
        if (client != null) client.Close();
        client = new UDPClient(localPort, serverURL, serverPort);
        RegisterMessageSend obj = new RegisterMessageSend(maxPlayers);
        client.SendRequest(ObjToBytes<RegisterMessageSend>(obj));
    }

    public void DisconnectFromServer(){
        connectedToServer = false;
        gameCode = null;
        ConnectionEvent connectionEventArgs = new ConnectionEvent(gameCode);
        EventDictionary[UDPEventType.DISCONNECT]?.Invoke(this, connectionEventArgs);
        if (client != null) client.Close();
    }

    public InputState[] GetPlayerInputs(){
        InputState[] result = new InputState[PlayerInputs.Length];
        for(int i=0; i<PlayerInputs.Length; i++){
            result[i] = PlayerInputs[i].Clone();
            PlayerInputs[i].NewFrame();
        }
        return result;
    } 

    public void HandleMsgEvents() {
        if(client == null) return;
        var receiveQueue = client.receiveQueue;
        byte[] data;
        while (receiveQueue.TryPeek(out data)){
            receiveQueue.TryDequeue(out data);
            HandleMsgData(data);
        }
        return;
    }

    private void HandleMsgData(byte[] data){
        CommType commType = (CommType) data[0];
        data = data.Skip(1).ToArray();
        switch(commType) {

            case CommType.InputData:
                InputData inputData = new InputData(data);
                int playerNum = inputData.PlayerNum;
                InputState newState = PlayerInputs[playerNum].ProcessNewData(inputData);
                InputEvent inpEventArgs = new InputEvent(newState, playerNum);
                EventDictionary[UDPEventType.INPUT]?.Invoke(this, inpEventArgs);
                break;

            case CommType.ConnectionSuccessful:
                MsgWithGameCode connection = BytesToObj<MsgWithGameCode>(data);
                gameCode = connection.gameCode;
                connectedToServer = true;
                ConnectionEvent connEventArgs = new ConnectionEvent(gameCode);
                EventDictionary[UDPEventType.CONNECTION]?.Invoke(this, connEventArgs);
                StartPingThreads();                
                break;

            case CommType.ConnectionUnsuccessful:
                ConnectionEvent connectionEventArgs = new ConnectionEvent(gameCode);
                EventDictionary[UDPEventType.CONNECTION_FAILED]?.Invoke(this, connectionEventArgs);
                break;

            case CommType.ConnectionClosed:
                MsgWithGameCode connectionClosed = BytesToObj<MsgWithGameCode>(data);
                if (gameCode == connectionClosed.gameCode){
                    DisconnectFromServer();
                }
                break;

            case CommType.PlayerJoined:
                MsgWithPlayerNum playerJoined = BytesToObj<MsgWithPlayerNum>(data);
                currentPlayers = playerJoined.currentPlayers;
                PlayerEvent pjEventArgs = new PlayerEvent(playerJoined.playerNum, currentPlayers);
                EventDictionary[UDPEventType.PLAYER_JOINED]?.Invoke(this, pjEventArgs);
                break;

            case CommType.PlayerLeft:
                MsgWithPlayerNum playerLeft = BytesToObj<MsgWithPlayerNum>(data);
                currentPlayers = playerLeft.currentPlayers;
                PlayerEvent plEventArgs = new PlayerEvent(playerLeft.playerNum, playerLeft.currentPlayers);
                EventDictionary[UDPEventType.PLAYER_LEFT]?.Invoke(this, plEventArgs);
                break;

            case CommType.Ping:
                MsgWithGameCode ping = BytesToObj<MsgWithGameCode>(data);
                if (gameCode == ping.gameCode) {
                    pinged = true;
                    PingEvent pingEventArgs = new PingEvent();
                    EventDictionary[UDPEventType.PING]?.Invoke(this, pingEventArgs);
                }
                break;

            default: 
                break;

        }
        return;
    }

    private void StartPingThreads(){
        pingSendThread = new Thread(PingServer);
        pingSendThread.Start();
        pingReceiveThread = new Thread(CheckPing);
        pingReceiveThread.Start();
    }

    private void PingServer(){
        while(connectedToServer){
            Task.Delay(1000).Wait();
            PingMessageSend obj = new PingMessageSend(gameCode);
            client.SendRequest(ObjToBytes<PingMessageSend>(obj));
        }
    }

    private void CheckPing(){
        while(connectedToServer){
            Task.Delay(5000).Wait();
            if(!pinged){
                DisconnectFromServer();
                connectedToServer = false;
                gameCode = null;
            }
            pinged = false;
        }
    }

    public EventHandler AddEventHandler(UDPEventType type, EventHandler handler){
        EventDictionary[type] += handler;
        return handler;
    }

    public void RemoveEventHandler(UDPEventType eventType, EventHandler handler){
        EventDictionary[eventType] -= handler;
    }

    public void ClearEventType(UDPEventType eventType){
        EventDictionary[eventType] = null;
    }

    private T BytesToObj<T>(byte[] data){
        string str = encoding.GetString(data);
        return JsonUtility.FromJson<T>(str);
    }

    private byte[] ObjToBytes<T>(T obj){
        string str = JsonUtility.ToJson(obj);
        return encoding.GetBytes(str);
    }
    
}



}





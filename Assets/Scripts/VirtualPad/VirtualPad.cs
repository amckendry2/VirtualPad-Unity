using System;
using VirtualPadUtils;

public class VirtualPad {
    

    private UDPHandler udpHandler;

    //CONSTRUCTOR==========
    //=====================
    public VirtualPad(int localPort, string serverURL, int serverPort, int maxPlayers){
        udpHandler = new UDPHandler(localPort, serverURL, serverPort, maxPlayers);
    }

    //API==================
    //vvvvvvvvvvvvvvvvvvvvv
    public string GameCode {get {return udpHandler.gameCode;}}
    public bool ConnectedToServer { get { return udpHandler.connectedToServer;}}
    public int MaxPlayers {get {return udpHandler.maxPlayers;}}
    public int CurrentPlayers { get { return udpHandler.currentPlayers;}}

    public void FrameHook(){
        udpHandler.HandleMsgEvents();
    }
    
    public InputState[] GetPlayerInputs(){ //**must be called once per frame**
        return udpHandler.GetPlayerInputs();
    }

    public EventHandler AddEventHandler<T>(UDPEventType type, Action<T> callback) where T : EventArgs {
        EventHandler handler = new EventHandler((object obj, EventArgs args) => callback((T) args));
        return udpHandler.AddEventHandler(type, handler);
    }

    public void RemoveEventHandler(UDPEventType eventType, EventHandler handler){
        udpHandler.RemoveEventHandler(eventType, handler);
    }

    public void ClearEventType(UDPEventType eventType){
        udpHandler.ClearEventType(eventType);
    }

    public void ConnectToServer(){
        udpHandler.RegisterGameWithServer();
    }

    public void DisconnectFromServer(){
        udpHandler.DisconnectFromServer();
    }
}


//For use with adding/removing event Handlers
public enum UDPEventType {
    INPUT = 0,
    CONNECTION = 1,
    CONNECTION_FAILED = 2,
    DISCONNECT = 3,
    PLAYER_JOINED = 4,
    PLAYER_LEFT = 5,
    PING = 6
}


//EVENTARGS CLASSES====
//=====================

//Passed to Connection event handlers
public class ConnectionEvent : EventArgs{
    public readonly string gameCode;
    public readonly bool connectedToServer;
    public ConnectionEvent(string code){
        gameCode = code;
    }
}

//Passed to Player event handlers
public class PlayerEvent : EventArgs{
    public readonly int playerNum;
    public readonly int newPlayerCount;
    public PlayerEvent(int pNum, int pCount){
        playerNum = pNum;
        newPlayerCount = pCount;
    }
}

//Passed to Input event handlers
public class InputEvent : EventArgs{
    public readonly InputState inputState;
    public readonly int playerNum;
    public InputEvent(InputState iState, int pNum){
        inputState = iState;
        playerNum = pNum;
    }
}

//Passed to Ping event handlers
public class PingEvent : EventArgs{
    //
}
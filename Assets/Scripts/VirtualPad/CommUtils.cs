using System;

namespace VirtualPadUtils.CommUtils{

    public enum CommType : byte {
        InputData = 0,
        ConnectionUnsuccessful = 1,
        ConnectionSuccessful = 2,
        ConnectionClosed = 3,
        PlayerJoined = 4,
        PlayerLeft = 5,
        Ping = 6
    }

    public class RegisterMessageSend {
        public string type = "register";
        public int players;
        public RegisterMessageSend(int p){
            players = p;
        }
    };

    public class PingMessageSend {
        public string type = "ping";
        public string gameCode;

        public PingMessageSend(string code){
            gameCode = code;
        }
    }

    public class MsgWithGameCode{
            public string gameCode;
        }

    public class MsgWithPlayerNum{
        public int playerNum;
        public int currentPlayers;
    }

    public class InputData {

        public int PlayerNum;
        public int StickDir;
        public int StickMag;
        public bool StickActive;
        public bool AButtonDown;
        public bool BButtonDown;

        public InputData(byte[] buf) {
            UInt16[] arr = new UInt16[buf.Length];
            Buffer.BlockCopy(buf, 0, arr, 0, buf.Length);
            PlayerNum = arr[0];
            StickDir = arr[1];
            StickMag = arr[2];
            StickActive = arr[3] == 1;
            AButtonDown = arr[4] == 1;
            BButtonDown = arr [5] == 1;
        }
        
    }

}



using UnityEngine;
using UnityEngine.UI;

public class VPTest : MonoBehaviour
{

    [SerializeField]
    private int localPort;
    [SerializeField]
    private string serverURL;
    [SerializeField]
    private int serverPort;
    [SerializeField]
    private bool usingLocalhost; 
    [SerializeField]
    private int maxPlayers;

    public GameObject controllerDisplay;
    public Text gameCodeText;
    
    private string gameCode;
    private VirtualPad virtualPad;
    private Controller[] controllers;
    private InputState[] inputStates;

    private static Vector3[] controllerPos  = new Vector3[]{
        new Vector3(1, -6, 1),
        new Vector3(1, .5f, 1),
        new Vector3(5.5f, -3, 1),
        new Vector3(-4, -3, 1)
    };

    private struct Controller{
        public GameObject controllerObject;
        public ControllerDisplay controllerDisplay;
        public Controller(GameObject cd){
            controllerObject = cd;
            controllerDisplay = cd.GetComponent<ControllerDisplay>();
        }
    }

    void Start()
    {    
        serverURL = usingLocalhost? "localhost" : serverURL;
        controllers = new Controller[maxPlayers];

        for(int i = 0; i < maxPlayers; i++){
            GameObject cd = Instantiate(
                controllerDisplay, 
                controllerPos[i], 
                Quaternion.identity
            );
            cd.SetActive(false);
            controllers[i] = new Controller(cd);
        }

        virtualPad = new VirtualPad(localPort, serverURL, serverPort, maxPlayers);

        virtualPad.AddEventHandler<ConnectionEvent>(UDPEventType.CONNECTION, args => {
            gameCode = args.gameCode;
        });
        virtualPad.AddEventHandler<ConnectionEvent>(UDPEventType.DISCONNECT, _ => {
            gameCode = null;
        });
        virtualPad.AddEventHandler<PlayerEvent>(UDPEventType.PLAYER_JOINED, args => {
            controllers[args.playerNum].controllerObject.SetActive(true);
        });
        virtualPad.AddEventHandler<PlayerEvent>(UDPEventType.PLAYER_LEFT, args => {
            controllers[args.playerNum].controllerObject.SetActive(false);
        });

    }

    void Update()
    {
        virtualPad.FrameHook(); //call at the beginning of each frame
        gameCodeText.text = gameCode;
        inputStates = virtualPad.GetPlayerInputs();
        for (int i = 0; i < maxPlayers; i++){
         controllers[i].controllerDisplay.UpdateController(inputStates[i]);
        }
    }

    public void ConnectToServer(){
        virtualPad.ConnectToServer();
    }

    void OnApplicationQuit(){
        virtualPad.DisconnectFromServer();
    }

    

    
}

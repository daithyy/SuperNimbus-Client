using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;    

    public NakamaManager Nakama;

    [Header("User Management")]
    public GameObject Canvas;

    [HideInInspector]
    public UIManager UI;

    public User User;

    [HideInInspector]
    public string NakamaIp;

    [Header("Spawn Management")]
    public GameObject EntityManager;

    [HideInInspector]
    public EntityManager Spawn;

    public string Token;

    private bool connectionFail = true;

    private bool connectionSuccess = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        Nakama = new NakamaManager();

        UI = Canvas.GetComponent<UIManager>();
        Spawn = EntityManager.GetComponent<EntityManager>();
    }

    private void Update()
    {
        if (!connectionFail && UI.InfoField != null)
        {
            UI.SendMainMenuMessage(Constants.UI.ConnectionFailedGameServer);
            connectionFail = true;
        }

        if (connectionSuccess)
        {
            connectionSuccess = false;
            UI.CreatePlayerHud();
        }
    }

    private async void OnApplicationQuit()
    {
        await Nakama.Disconnect();
    }

    private void StoreDataFromUI()
    {
        User = new User(UI.EmailField.text, UI.UsernameField.text, UI.PasswordField.text);
        Nakama.User = User;

        NakamaIp = UI.NakamaIp.text;
    }

    #region NAKAMA 

    public async void NakamaConnect()
    {
        StoreDataFromUI();
        
        bool connected = await Nakama.Connect();

        if (connected)
        {
            UI.SendMainMenuMessage($"<color=#00FF00>{User.Username}</color> has logged in to Nakama successfully!");
            NakamaFindMatch();
        }
        else
        {
            UI.SendMainMenuMessage(Constants.UI.ConnectionFailedNakama);
        }

        Nakama.Socket.ReceivedMatchmakerMatched += m => ThreadManager.ExecuteOnMainThread(() => OnMatchFound(m));
    }

    public async void NakamaRegister()
    {
        StoreDataFromUI();

        bool isRegistered = await Nakama.Register();

        if (isRegistered)
        {
            UI.SendMainMenuMessage($"<color=#35BAFD>{User.Username}</color> has been registered to Nakama successfully!");
        }
        else
        {
            UI.SendMainMenuMessage(Constants.UI.RegistrationFailed);
        }
    }

    private async void NakamaFindMatch()
    {
        UI.LoadingIcon.SetActive(true);
        UI.ButtonContainer.SetActive(false);

        await Nakama.FindMatch();
    }

    #endregion

    #region EVENTS

    private void OnEnable()
    {
        EventManager.onConnectToServer += OnGameServerConnection;
    }

    private void OnDisable()
    {
        EventManager.onConnectToServer -= OnGameServerConnection;

        if (Nakama.Socket != null)
        {
            Nakama.Socket.ReceivedMatchmakerMatched -= OnMatchFound;
        }
    }

    private void OnGameServerConnection(bool isConnected)
    {
        if (!isConnected)
        {
            connectionFail = isConnected;
        }
        else
        {
            connectionSuccess = isConnected;
        }
    }

    public async void OnMatchFound(Nakama.IMatchmakerMatched matchmakerMatched)
    {
        Token = matchmakerMatched.Token;

        await Nakama.JoinMatch(matchmakerMatched);
        await Nakama.StoreAuth(Token);

        UI.LoadingIcon.SetActive(false);

        Client.Instance.ConnectToServer(UI.ServerIp.text, int.Parse(UI.ServerPort.text));
    }

    #endregion
}

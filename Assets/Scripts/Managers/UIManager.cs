using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public NakamaManager Nakama;

    public GameObject MenuCamera;

    [Header("Main Menu UI")]
    [Header("Game Server Input Fields")]
    public InputField ServerIp;

    public InputField ServerPort;

    [Header("Nakama Input Fields")]
    public InputField NakamaIp;

    public InputField NakamaPort;

    [Header("User Input Fields")]
    public InputField EmailField;    

    public InputField UsernameField;

    public InputField PasswordField;

    public Text InfoField;

    [Header("Game Display UI")]
    [Header("HUD")]
    public GameObject ChatInputPanel;

    public GameObject ChatDisplayLog;

    public GameObject PlayerInfo;

    public GameObject ServerInfo;

    [HideInInspector]
    public InputField ChatInputField;

    private GameObject StartMenu;

    private MessageLog[] logs;

    private bool connectionFail = true;

    private bool connectionSuccess = false;

    public async void NakamaConnect()
    {
        await Nakama.Connect(EmailField.text, UsernameField.text, PasswordField.text);
    }

    public async void NakamaRegister()
    {
        bool userCreated = await Nakama.Register(EmailField.text, UsernameField.text, PasswordField.text);

        if (userCreated)
        {
            SendConnectionMessage($"<color=#00FF00>{UsernameField.text}</color> has been registered to Nakama successfully!");
        }
        else
        {
            SendConnectionMessage(Constants.UI.RegistrationFailed);
        }
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.visible = toggle;
        Cursor.lockState = toggle ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public void ToggleChat(bool toggle)
    {
        ToggleCursor(toggle);
        ChatInputPanel.SetActive(toggle);
        ChatDisplayLog.SetActive(!toggle);
    }

    public void SendMessage(Message message)
    {
        logs[(int)ChatLogType.Input].ReceiveMessage(message);
        logs[(int)ChatLogType.Display].ReceiveMessage(message);
    }

    public void SendConnectionMessage(string message)
    {
        InfoField.text = message;
    }

    private enum ChatLogType
    {
        Input,
        Display,
    }

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

        FillInputData();

        StartMenu = transform.GetChild(0).gameObject;

        logs = new MessageLog[2];

        CreateChat();
        CreateChatLog();
    }

    private void Update()
    {
        if (!connectionFail && InfoField != null)
        {
            SendConnectionMessage(Constants.UI.ConnectionFailed);
        }

        if (connectionSuccess)
        {
            connectionSuccess = false;
            CreateRemainingUI();
        }
    }

    private void OnEnable()
    {
        EventManager.onServerConnect += ConnectionCheck;
    }

    private void OnDisable()
    {
        EventManager.onServerConnect -= ConnectionCheck;
    }

    private void ConnectionCheck(bool isConnected)
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

    private void DisableMainMenu()
    {
        ToggleCursor(false);
        StartMenu.SetActive(false);
        UsernameField.interactable = false;
    }

    private void CreateChat()
    {
        ChatInputPanel = Instantiate(ChatInputPanel, transform);
        ChatInputField = ChatInputPanel.GetComponentInChildren<ChatInputField>().MessageInput;
        ChatInputPanel.SetActive(false);
        logs[(int)ChatLogType.Input] = ChatInputPanel.GetComponentInChildren<MessageLog>();
    }

    private void CreateChatLog()
    {
        ChatDisplayLog = Instantiate(ChatDisplayLog, transform);
        ChatDisplayLog.SetActive(false);
        logs[(int)ChatLogType.Display] = ChatDisplayLog.gameObject.GetComponentInChildren<MessageLog>();
    }

    private void CreateRemainingUI()
    {
        DisableMainMenu();

        ChatDisplayLog.SetActive(true);

        ServerInfo = Instantiate(ServerInfo, transform);
        
        PlayerInfo = Instantiate(PlayerInfo, transform);

        MenuCamera.SetActive(false);
    }

    private void FillInputData()
    {
        ServerIp.text = Constants.IpDefault;
        ServerPort.text = Constants.Port.ToString();

        NakamaIp.text = Constants.IpDefault;
        NakamaPort.text = Constants.Port.ToString();

        if (PlayerPrefs.HasKey(NakamaManager.EmailIdentifierPrefName))
        {
            EmailField.text = PlayerPrefs.GetString(NakamaManager.EmailIdentifierPrefName);
        }
    }
}

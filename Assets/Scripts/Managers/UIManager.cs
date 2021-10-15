using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject MenuCamera;

    public InputField IpAddress;

    public InputField Port;

    public InputField UsernameField;

    public GameObject ChatInputPanel;

    public GameObject ChatDisplayLog;

    public GameObject PlayerInfo;

    public GameObject ServerInfo;

    public Text InfoField;

    [HideInInspector]
    public InputField ChatInputField;

    private GameObject StartMenu;

    private MessageLog[] logs;

    private bool connectionFail = true;

    private bool connectionSuccess = false;

    public void ConnectToServer()
    {
        Client.Instance.ConnectToServer(IpAddress.text, Port.text);
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
        IpAddress.text = Client.Instance.Ip;
        Port.text = Client.Instance.Port.ToString();

        StartMenu = transform.GetChild(0).gameObject;

        logs = new MessageLog[2];

        CreateChat();
        CreateChatLog();
    }

    private void Update()
    {
        if (!connectionFail && InfoField != null)
        {
            InfoField.text = "<color=#FF0041>Could not connect to IP and port entered. Please try again.</color>";
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
}

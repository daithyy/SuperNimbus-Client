using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenuCamera;

    [Header("Main Menu UI")]
    public GameObject LoadingIcon;

    public GameObject ButtonContainer;

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

    [Header("Game UI Prefabs")]
    [Header("HUD")]
    public GameObject ChatInputPanel;

    public GameObject ChatDisplayLog;

    public GameObject PlayerInfo;

    public GameObject ServerInfo;

    [HideInInspector]
    public InputField ChatInputField;

    private GameObject MainMenu;

    private MessageLog[] logs;

    private enum ChatLogType
    {
        Input,
        Display,
    }

    void Start()
    {
        PrefillInputData();

        MainMenu = transform.GetChild(0).gameObject;

        LoadingIcon.SetActive(false);

        logs = new MessageLog[2];

        CreateChat();
        CreateChatLog();
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

    public void SendMainMenuMessage(string message)
    {
        InfoField.text = message;
    }

    public void CreatePlayerHud()
    {
        DisableMainMenu();

        ChatDisplayLog.SetActive(true);

        ServerInfo = Instantiate(ServerInfo, transform);

        PlayerInfo = Instantiate(PlayerInfo, transform);

        MainMenuCamera.SetActive(false);
    }

    private void DisableMainMenu()
    {
        ToggleCursor(false);
        MainMenu.SetActive(false);

        ServerIp.interactable = false;
        ServerPort.interactable = false;
        NakamaIp.interactable = false;
        NakamaPort.interactable = false;
        EmailField.interactable = false;
        UsernameField.interactable = false;
        PasswordField.interactable = false;
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

    private void PrefillInputData()
    {
        ServerIp.text = Constants.IpDefault;
        ServerPort.text = Constants.Port.ToString();

        NakamaIp.text = Constants.IpDefault;
        NakamaPort.text = Constants.Nakama.Port.ToString();

        if (PlayerPrefs.HasKey(NakamaManager.EmailIdentifierPrefName))
        {
            EmailField.text = PlayerPrefs.GetString(NakamaManager.EmailIdentifierPrefName);
        }
    }
}

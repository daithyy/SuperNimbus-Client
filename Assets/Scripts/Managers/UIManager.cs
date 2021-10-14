using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject MenuCamera;

    public GameObject StartMenu;

    public InputField UsernameField;

    public GameObject ChatInputPanel;

    public GameObject ChatDisplayLog;

    [HideInInspector]
    public InputField ChatInputField;

    private MessageLog[] logs;

    private enum ChatLogType
    {
        Display,
        Input,
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

    public void ConnectToServer()
    {
        ToggleCursor(false);
        StartMenu.SetActive(false);
        UsernameField.interactable = false;

        ChatInputPanel = Instantiate(ChatInputPanel, transform);
        ChatInputField = ChatInputPanel.GetComponentInChildren<ChatInputField>().MessageInput;

        Client.Instance.ConnectToServer();

        ChatInputPanel.SetActive(false);

        ChatDisplayLog = Instantiate(ChatDisplayLog, transform);
        ChatDisplayLog.SetActive(true);

        logs = new MessageLog[2];
        logs[0] = ChatInputPanel.GetComponentInChildren<MessageLog>();
        logs[1] = ChatDisplayLog.gameObject.GetComponentInChildren<MessageLog>();

        MenuCamera.SetActive(false);
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

    public void ReceiveMessage(Message message)
    {
        logs[0].ReceiveMessage(message);
        logs[1].ReceiveMessage(message);
    }
}

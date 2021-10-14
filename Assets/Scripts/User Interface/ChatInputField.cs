using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class ChatInputField : MonoBehaviour
{
    public InputField MessageInput;

    public string ChannelName { get; set; }

    void Awake()
    {
        MessageInput = GetComponentInChildren<InputField>();
        MessageInput.onEndEdit.AddListener(str => Submit());
    }

    void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("No EventSystem detected! Please make sure your scene is correctly set up to handle UI input.");
        }
    }

    public void Submit()
    {
        var text = MessageInput.text;

        if (text.Length > 0)
        {
            SendController.MessageClient(text);

            MessageInput.text = string.Empty;
            MessageInput.ActivateInputField();
            MessageInput.Select();

            UIManager.Instance.ToggleChat(!PlayerController.LockInput);
            PlayerController.LockInput = !PlayerController.LockInput;
        }
    }
}

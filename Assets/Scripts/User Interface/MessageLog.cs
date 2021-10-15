using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MessageLog : MonoBehaviour
{
    private readonly Queue<Message> messageEntries = new Queue<Message>();

    private Text textField;

    private bool refresh;

    private int maxLines = 15;

    private int charactersPerRowEstimate = 35;

    public void ReceiveMessage(Message msg)
    {
        if (msg.Text.Length > 0)
        {
            messageEntries.Enqueue(msg);
        }
        refresh = true;
    }

    private void Awake()
    {
        textField = GetComponent<Text>();
    }

    private void Start()
    {
        textField.text = string.Empty;
    }

    private void Update()
    {
        if (refresh)
        {
            refresh = false;
            UpdateChatLog();
        }
    }

    private void UpdateChatLog()
    {
        lock (messageEntries)
        {
            var messages = messageEntries
                .OrderBy(e => e.ReceivedAt)
                .ToList();

            while (CalculateLines(messages) > maxLines)
            {
                messageEntries.Dequeue();
                messages = messageEntries.ToList();
            }

            RenderText(messages);
        }
    }

    private int CalculateLines(List<Message> messages)
    {
        var lines = 0;
        for (var i = 0; i < messages.Count; i++)
        {
            var messagesParts = messages[i].Text.Split('\n');
            for (var j = 0; j < messagesParts.Length; j++)
            {
                var part = messagesParts[j];
                lines += (int)Math.Ceiling((float)part.Length / charactersPerRowEstimate);
            }
        }

        return lines;
    }

    private void RenderText(List<Message> messages)
    {
        var sb = new StringBuilder();
        foreach (var message in messages)
        {
            switch (message.Type)
            {
                case MessageType.ServerMessage:
                    sb.Append($"\n<size=18>{message.ReceivedAt.ToString("T", CultureInfo.CurrentCulture)}</size> <i><color=#66D9EF>{message.Username}</color> > {message.Text}</i>");
                    break;
                case MessageType.GameMessage:
                    sb.Append($"\n<size=18>{message.ReceivedAt.ToString("T", CultureInfo.CurrentCulture)}</size> <i><color=#00FF00>{message.Username}</color> > {message.Text}</i>");
                    break;
                case MessageType.PlayerMessage:
                    sb.Append($"\n<size=18>{message.ReceivedAt.ToString("T", CultureInfo.CurrentCulture)}</size> <color=#FFF545>{message.Username}</color> > {message.Text}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        textField.text = sb.ToString();
    }
}

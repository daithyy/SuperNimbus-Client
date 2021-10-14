using System;

public enum MessageType
{
    ServerMessage,
    PlayerMessage
}

public class Message
{
    public string Username;
    
    public string Text;
    
    public DateTime ReceivedAt;
    
    public MessageType Type;

    public Message(int id, string message, DateTime datetime)
    {
        Username = (id == Constants.ServerId) ? Constants.ServerName.ToUpper() : GameManager.Players[id].Username;
        Text = message;
        ReceivedAt = datetime;
        Type = (id == Constants.ServerId) ? MessageType.ServerMessage : MessageType.PlayerMessage;
    }
}

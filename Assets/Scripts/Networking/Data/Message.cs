using System;

public enum MessageType
{
    ServerMessage,
    GameMessage,
    PlayerMessage
}

public class Message
{
    public string Username;
    
    public string Text;
    
    public DateTime ReceivedAt;
    
    public MessageType Type;

    public Message(int id, string message, string datetime)
    {
        Username = (id == Constants.ServerId) ? Constants.ServerName.ToUpper() : EntityManager.Players[id].Username;
        Text = message;
        ReceivedAt = DateTime.TryParse(datetime, out ReceivedAt) ? ReceivedAt : new DateTime();
        Type = id switch
        {
            Constants.ServerId => MessageType.ServerMessage,
            Constants.GameId => MessageType.GameMessage,
            _ => MessageType.PlayerMessage,
        };
    }
}

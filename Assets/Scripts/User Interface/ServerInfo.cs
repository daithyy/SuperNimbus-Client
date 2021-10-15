public class ServerInfo : TextInfo
{
    public string IpAddress;

    public string Port;

    public override void Update()
    {
        textField.text =
            $"\n -- <b>Server Info</b> -- " +
            $"\nIP Address: <color=#FFF545>{Client.Instance.Ip}</color>" +
            $"\nPort: <color=#FFF545>{Client.Instance.Port}</color>";
    }
}

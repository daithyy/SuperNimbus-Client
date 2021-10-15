public class PlayerInfo : TextInfo
{
    private int playerId;

    private string username;

    private void OnEnable()
    {
        EventManager.onRetreivePlayerInfo += RetrieveInfo;
    }

    private void OnDisable()
    {
        EventManager.onRetreivePlayerInfo -= RetrieveInfo;
    }

    public override void Update()
    {
        if (GameManager.Players.ContainsKey(playerId))
        {
            textField.text =
            $"\n -- <b>Player:</b> <color=#00FF00>{playerId}</color> -- " +
            $"\nName: <color=#FFF545>{username}</color>" +
            $"\nCoins Collected: <color=#FFF545>{GameManager.Players[playerId].ItemCount}</color>";
        }
    }

    private void RetrieveInfo(int id, string name)
    {
        playerId = id;
        username = name;
    }
}

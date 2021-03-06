using Nakama;
using System.Threading.Tasks;
using UnityEngine;
using static Constants.Nakama;

public class NakamaManager
{
    public User User;

    private IClient client;

    private ISession session;

    public ISocket Socket;

    public IMatch CurrentMatch;

    private const string SessionPrefName = "nakama.session";

    public const string EmailIdentifierPrefName = "nakama.emailUniqueIdentifier";

    private string currentMatchmakingTicket;

    /// <summary>
    /// Registers to the Nakama server providing email, username and password.
    /// </summary>
    public async Task<bool> Register()
    {
        if (client == null)
        {
            client = new Nakama.Client(Scheme, GameManager.Instance.NakamaIp, Port, ServerKey, UnityWebRequestAdapter.Instance);
        }

        string savedEmail = PlayerPrefs.GetString(EmailIdentifierPrefName);

        if (User.Email != savedEmail)
        {
            PlayerPrefs.SetString(EmailIdentifierPrefName, User.Email);
        }

        ISession session = await client.AuthenticateEmailAsync(User.Email, User.Password, User.Username);

        return session.Created;
    }

    /// <summary>
    /// Connects to the Nakama server using email authentication and opens socket for realtime communication.
    /// </summary>
    public async Task<bool> Connect()
    {
        if (client == null)
        {
            client = new Nakama.Client(Scheme, GameManager.Instance.NakamaIp, Port, ServerKey, UnityWebRequestAdapter.Instance);
        }

        var authToken = PlayerPrefs.GetString(SessionPrefName);
        if (!string.IsNullOrEmpty(authToken))
        {
            var session = Session.Restore(authToken);
            if (!session.IsExpired)
            {
                this.session = session;
            }
        }

        if (session == null)
        {
            string savedEmail = PlayerPrefs.GetString(EmailIdentifierPrefName);

            if (User.Email != savedEmail)
            {
                PlayerPrefs.SetString(EmailIdentifierPrefName, User.Email);
            }

            session = await client.AuthenticateEmailAsync(User.Email, User.Password, User.Username, create: false);

            PlayerPrefs.SetString(SessionPrefName, session.AuthToken);
        }

        Socket = client.NewSocket();
        await Socket.ConnectAsync(session, true);

        return Socket.IsConnected;
    }

    /// <summary>
    /// Starts looking for a match with a given number of minimum players.
    /// </summary>
    public async Task FindMatch(int minPlayers = 2, int maxPlayers = 8)
    {
        IMatchmakerTicket matchmakerTicket = await Socket.AddMatchmakerAsync("*", minPlayers, maxPlayers);
        currentMatchmakingTicket = matchmakerTicket.Ticket;
    }

    public async Task JoinMatch(IMatchmakerMatched matchmakerMatched)
    {
        CurrentMatch = await Socket.JoinMatchAsync(matchmakerMatched);
    }

    /// <summary>
    /// Cancels the current matchmaking request.
    /// </summary>
    public async Task CancelMatchmaking()
    {
        await Socket.RemoveMatchmakerAsync(currentMatchmakingTicket);
    }

    public async Task StoreAuth(string data)
    {
        Token obj = new Token() { token = data };
        string token = JsonUtility.ToJson(obj);

        await client.WriteStorageObjectsAsync(session, new WriteStorageObject[]
        {
            new WriteStorageObject()
            {
                Collection = "matchToken",
                Key = CurrentMatch.Id,
                Value = token
            }
        });
    }

    /// <summary>
    /// Quits the current match.
    /// </summary>
    /// <returns></returns>
    public async Task Disconnect()
    {
        if (CurrentMatch != null)
        {
            await Socket.LeaveMatchAsync(CurrentMatch);
        }
    }
}
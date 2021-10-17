using Nakama;
using System.Threading.Tasks;
using UnityEngine;
using static Constants.Nakama;

public class NakamaManager
{
    public IClient Client;

    public ISession Session;

    public ISocket Socket;

    private const string SessionPrefName = "nakama.session";

    public const string EmailIdentifierPrefName = "nakama.emailUniqueIdentifier";

    private string currentMatchmakingTicket;

    private string currentMatchId;

    /// <summary>
    /// Connects to the Nakama server using email authentication and opens socket for realtime communication.
    /// </summary>
    public async Task Connect(string email, string username, string password)
    {
        // Connect to the Nakama server.
        Client = new Nakama.Client(Scheme, UIManager.Instance.ServerIp.text, Port, ServerKey, UnityWebRequestAdapter.Instance);

        // Attempt to restore an existing user session.
        var authToken = PlayerPrefs.GetString(SessionPrefName);
        if (!string.IsNullOrEmpty(authToken))
        {
            var session = Nakama.Session.Restore(authToken);
            if (!session.IsExpired)
            {
                Session = session;
            }
        }

        // If we weren't able to restore an existing session, authenticate to create a new user session.
        if (Session == null)
        {
            string savedEmail = PlayerPrefs.GetString(EmailIdentifierPrefName);

            if (email != savedEmail)
            {
                // Store the email identifier to ensure we retrieve the same one each time from now on.
                PlayerPrefs.SetString(EmailIdentifierPrefName, email);
            }

            // Use Nakama email authentication to create a new session using the provided email
            Session = await Client.AuthenticateEmailAsync(email, password, username, create: false);

            // Store the auth token that comes back so that we can restore the session later if necessary.
            PlayerPrefs.SetString(SessionPrefName, Session.AuthToken);
        }

        // Open a new Socket for realtime communication.
        Socket = Client.NewSocket();
        await Socket.ConnectAsync(Session, true);
    }

    /// <summary>
    /// Registers to the Nakama server providing email, username and password.
    /// </summary>
    public async Task<bool> Register(string email, string username, string password)
    {
        // Connect to the Nakama server.
        Client = new Nakama.Client(Scheme, UIManager.Instance.ServerIp.text, Port, ServerKey, UnityWebRequestAdapter.Instance);

        string savedEmail = PlayerPrefs.GetString(EmailIdentifierPrefName);

        if (email != savedEmail)
        {
            // Store the email identifier to ensure we retrieve the same one each time from now on.
            PlayerPrefs.SetString(EmailIdentifierPrefName, email);
        }

        // Use Nakama email authentication to register a new user
        ISession user = await Client.AuthenticateEmailAsync(email, password, username);

        return user.Created;
    }

    /// <summary>
    /// Starts looking for a match with a given number of minimum players.
    /// </summary>
    public async Task FindMatch(int minPlayers = 2)
    {
        // Add this client to the matchmaking pool and get a ticket.
        var matchmakerTicket = await Socket.AddMatchmakerAsync("*", minPlayers, minPlayers);
        currentMatchmakingTicket = matchmakerTicket.Ticket;
    }

    /// <summary>
    /// Cancels the current matchmaking request.
    /// </summary>
    public async Task CancelMatchmaking()
    {
        await Socket.RemoveMatchmakerAsync(currentMatchmakingTicket);
    }
}
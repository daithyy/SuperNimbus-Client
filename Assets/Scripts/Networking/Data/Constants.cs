﻿
public class Constants
{
    public const string IpDefault = ipLocal;

    public const int Port = 26950;

    public const int BufferConstant = 4096;

    public const int ServerId = -1;

    public const int GameId = -2;

    public const string ServerName = "Server";

    public const string GameName = "Game";

    private const string ipLocal = "127.0.0.1";

    private const string ipRemote = "185.108.129.11";

    public class Nakama
    {
        public const int Port = 7350;

        public const string Scheme = "http";

        public const string ServerKey = "defaultkey";
    }

    public class UI
    {
        public const string ConnectionFailed = "<color=#FF0041>Could not connect to Game Server IP and port entered. Please try again.</color>";
        public const string EmailAlreadyExist = "A user was found with this email address, please register with a different email address";
        public const string RegistrationFailed = "<color=#FF0041>Failed to register user, please check your connection and that your credentials are valid.</color>";
    }
}

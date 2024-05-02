namespace MusicApp.Authentication
{
    public interface IAuthenticationModule
    {
        bool RegisterUser(string username, string password);
        bool AuthenticateUser(string username, string password);
    }
}


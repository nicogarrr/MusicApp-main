public interface IClientManager
{
    void InitializeUsersList();
    void AddUser(IClient client);
    void BroadcastMessage(string message);
    void BroadcastDisconnect(string uid);
}


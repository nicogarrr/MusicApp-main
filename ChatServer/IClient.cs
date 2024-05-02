public interface IClient
{
    string Username { get; set; }
    Guid UID { get; set; }
    void Process();
}

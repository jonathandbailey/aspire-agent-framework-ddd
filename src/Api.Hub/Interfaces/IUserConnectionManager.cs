namespace Api.Hub.Interfaces;

public interface IUserConnectionManager
{
    void AddConnection(Guid userId, string connectionId);
    void RemoveConnection(string connectionId);
    List<string> GetConnections(Guid userId);
}
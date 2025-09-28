namespace Domain.Common;

public abstract class BaseMessage
{
    public Guid Id { get; }

    public string Role { get; } 

    public string Content { get; private set; }

    public int Index { get; private set; }

    protected BaseMessage(Guid id, int index, string content, string role)
    {
        Verify.NotEmpty(id);
        Verify.NotNull(content);
        Verify.NotNullOrWhiteSpace(role);
        
        Id = id;
        Index = index;
        Content = content;
        Role = role;
    }

    public void Update(string content)
    {
        Content = content;
    }
}
namespace Domain;

public abstract class Message
{
    public Guid Id { get; }

    public string Role { get; } 

    public string Content { get; private set; }

    protected Message(Guid id, string content, string role)
    {
        Verify.NotEmpty(id);
        Verify.NotNull(content);
        Verify.NotNullOrWhiteSpace(role);
        
        Id = id;
        Content = content;
        Role = role;
    }

    public void Append(string partialContent)
    {
        Content += partialContent;
    }
}
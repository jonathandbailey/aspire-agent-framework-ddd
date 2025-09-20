using Domain.Conversations;

namespace Application.Interfaces;

public interface IAssistant
{
    public IAsyncEnumerable<string> StreamAsync(UserMessage userMessage);
}
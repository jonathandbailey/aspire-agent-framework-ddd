namespace Application.Dto;

public sealed record Conversation(Guid Id, string Name, List<ConversationThread> Threads);

public sealed record ConversationThread(Guid Id, List<ConversationExchange> Exchanges);

public sealed record ConversationExchange(
    Guid Id,
    int Index,
    ConversationMessage UserMessage,
    ConversationMessage AssistantMessage);

public sealed record ConversationMessage(Guid Id, string Role, string Content);

public sealed record ConversationSummaryItem(Guid Id, string Title);

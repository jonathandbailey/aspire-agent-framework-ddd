namespace Application.Dto;

public sealed record Conversation(Guid Id, string Name, Guid CurrentThread, List<ConversationThread> Threads);

public sealed record ConversationThread(Guid Id, List<ConversationTurn> Turns);

public sealed record ConversationTurn(
    Guid Id,
    int Index,
    ConversationMessage UserMessage,
    ConversationMessage AssistantMessage);

public sealed record ConversationMessage(Guid Id, string Role, string Content);

public sealed record ConversationSummaryItem(Guid Id, string Title);

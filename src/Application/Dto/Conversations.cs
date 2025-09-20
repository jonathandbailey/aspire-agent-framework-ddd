namespace Application.Dto;

public sealed record Conversation(Guid Id, string Name, Guid CurrentThread, List<ConversationThread> Threads);

public sealed record ConversationThread(Guid Id, List<ConversationMessage> Messages);

public sealed record ConversationMessage(Guid Id, string Role, string Content);

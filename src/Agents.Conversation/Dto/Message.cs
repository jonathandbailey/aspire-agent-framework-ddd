namespace Agents.Conversation.Dto;

public sealed record Message(Guid Id, int Index, string Content, string Role);
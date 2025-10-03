namespace Agents.Infrastructure.Dto;

public record ConversationAgentMessage(Guid UserId, Guid ExchangeId, Guid ConversationId, List<Message> Messages);

public record ConversationStreamingMessage(Guid UserId, string Message, Guid ConversationId, Guid ExchangeId);

public record ConversationDomainMessage(Guid UserId, string Content, Guid ConversationId, Guid ExchangeId);
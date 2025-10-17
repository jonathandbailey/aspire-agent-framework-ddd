using Domain.Conversations;

namespace Application.Dto;

public record ChatRequestDto (Guid Id,string Message, Guid ConversationId, Guid ExchangeId);

public record ChatResponseDto(Guid Id, string Message, Guid ConversationId);

public record ConversationStreamingMessage(Guid UserId, string Message, Guid ConversationId, Guid ExchangeId);

public record ConversationTitleUpdatedMessage(Guid UserId, Guid ConversationId, string Content);

public record ConversationTitleMessage(Guid UserId, Guid ConversationId, List<Message> Messages);

public record ConversationExchangeStartedMessage(Guid UserId, Guid ExchangeId, Guid ConversationId, List<Message> Messages);
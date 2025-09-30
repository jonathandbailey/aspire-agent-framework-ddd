namespace Api.Hub.Dto;

public record ConversationStreamingMessage(Guid UserId, string Message, Guid ConversationId, Guid ExchangeId);

public record ChatResponseDto(Guid Id, string Message, Guid ConversationId);
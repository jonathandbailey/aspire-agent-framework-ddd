namespace Api.Hub.Dto;

public record ConversationStreamingMessage(Guid UserId, string Message, Guid ConversationId, Guid ExchangeId);

public record ConversationTitleUpdatedMessage(Guid UserId, Guid ConversationId, string Content);

public record ChatResponseDto(Guid Id, string Message, Guid ConversationId);

public record ClientCommandUpdateConversationTitle(Guid ConversationId, string Title);
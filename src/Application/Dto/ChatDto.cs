namespace Application.Dto;

public record ChatRequestDto (Guid Id,string Message, Guid ConversationId, Guid ExchangeId);

public record ChatResponseDto(Guid Id, string Message, Guid ConversationId);
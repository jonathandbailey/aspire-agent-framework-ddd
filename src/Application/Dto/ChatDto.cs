namespace Application.Dto;

public record ChatRequestDto (Guid Id,string Message, Guid ConversationId);

public record ChatResponseDto(Guid Id, string Message, Guid ConversationId);
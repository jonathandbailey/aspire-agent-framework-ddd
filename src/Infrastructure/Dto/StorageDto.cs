namespace Infrastructure.Dto
{
    public record ConversationDto(Guid Id, Guid UserId, string Name, Guid CurrentThread, List<ConversationThreadDto> Threads);

    public sealed record ConversationThreadDto(Guid Id,int Index, List<ConversationTurnDto> Turns);

    public sealed record ConversationTurnDto(
        Guid Id,
        int Index,
        ConversationMessageDto UserMessage,
        ConversationMessageDto AssistantMessage);

    public sealed record ConversationMessageDto(Guid Id, int Index, string Role, string Content);
}

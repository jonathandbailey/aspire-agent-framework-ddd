namespace Infrastructure.Dto
{
    public record ConversationDto(Guid Id, Guid UserId, string Name, Guid CurrentThread, List<ConversationThreadDto> Threads);

    public sealed record ConversationThreadDto(Guid Id, List<ConversationMessageDto> Messages);

    public sealed record ConversationMessageDto(Guid Id, string Role, string Content);
}

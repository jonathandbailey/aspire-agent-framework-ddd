namespace Infrastructure.Dto
{
    public record ConversationDto(Guid Id, Guid UserId, string Name, Guid CurrentThread, List<ConversationThreadDto> Threads);

    public sealed record ConversationThreadDto(Guid Id,int Index, List<ConversationMessageDto> Messages);

    public sealed record ConversationMessageDto(Guid Id, int Index, string Role, string Content);
}

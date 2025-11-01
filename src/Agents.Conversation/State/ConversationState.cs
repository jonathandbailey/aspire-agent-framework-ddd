using Microsoft.Extensions.AI;

namespace Agents.Conversation.State;

public sealed record ConversationState(List<ChatMessage> Messages, Guid UserId, Guid ConversationId, Guid ExchangeId, string Title, string Response);


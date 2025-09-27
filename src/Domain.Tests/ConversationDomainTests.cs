using Domain.Conversations;
using Domain.Events;

namespace Domain.Tests
{
    public class ConversationDomainTests
    {
        [Fact]
        public void Constructor_InitializesWithUserId_CreatesInitialThread()
        {
            var userId = UserId.New();

            var conversation = new Conversation(userId);

            Assert.Equal(userId, conversation.UserId);
            Assert.NotEqual(Guid.Empty, conversation.Id);
            Assert.Empty(conversation.Name);
            Assert.Single(conversation.Threads);
        }

        [Fact]
        public void CreateConversationExchange_DelegatesToActiveThread()
        {
            var userId = UserId.New();
            var conversation = new Conversation(userId);

            var exchangeId = conversation.CreateConversationExchange();

            Assert.NotNull(exchangeId);
            Assert.Single(conversation.Threads.First().Exchanges);
        }

        [Fact]
        public void CompleteConversationExchange_AddsDomainEventAndUpdatesAssistantMessage()
        {
            var userId = UserId.New();
            var conversation = new Conversation(userId);

            var exchangeId = conversation.CreateConversationExchange();

            conversation.CompleteConversationExchange(exchangeId, "assistant content");

            var exchange = conversation.Threads.First().Exchanges.First(x => x.ExchangeId.Equals(exchangeId));
            Assert.Equal("assistant content", exchange.AssistantMessage.Content);

            Assert.Contains(conversation.DomainEvents, e => e is ConversationTurnEndedEvent);
        }

        [Fact]
        public void UpdateTitle_SetsNameAndAddsEvent()
        {
            var userId = UserId.New();
            var conversation = new Conversation(userId);

            conversation.UpdateTitle("New Title");

            Assert.Equal("New Title", conversation.Name);
            Assert.Contains(conversation.DomainEvents, e => e is ConversationTitleUpdatedEvent);
        }
    }
}

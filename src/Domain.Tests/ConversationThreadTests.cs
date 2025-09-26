using Domain.Conversations;
using Domain.Exceptions;

namespace Domain.Tests
{
    public class ConversationThreadTests
    {
        [Fact]
        public void StartConversationExchange_InvalidExchangeId_ThrowsExchangeNotFoundDomainException()
        {
            var thread = new ConversationThread(0);
            var invalidExchangeId = ExchangeId.New();

            var ex = Assert.Throws<ExchangeNotFoundDomainException>(() =>
                thread.StartConversationExchange("hello", invalidExchangeId));

            Assert.Contains(invalidExchangeId.Value.ToString(), ex.Message);
        }

        [Fact]
        public void Constructor_SetsIndex_IdAndEmptyExchanges()
        {
            var thread = new ConversationThread(2);

            Assert.Equal(2, thread.Index);
            Assert.NotEqual(Guid.Empty, thread.Id);
            Assert.Empty(thread.Exchanges);
        }

        [Fact]
        public void Constructor_WithParameters_SetsProperties()
        {
            var exchangeId = ExchangeId.New();
            var userMessage = new UserMessage("hi", 0);
            var assistantMessage = new AssistantMessage("reply", 1);
            var exchange = new ConversationExchange(exchangeId, 0, userMessage, assistantMessage);
            var exchanges = new List<ConversationExchange> { exchange };
            var id = Guid.NewGuid();

            var thread = new ConversationThread(id, 3, exchanges);

            Assert.Equal(3, thread.Index);
            Assert.Equal(id, thread.Id);
            Assert.Single(thread.Exchanges);
            Assert.Equal(exchangeId, thread.Exchanges.First().ExchangeId);
        }

        [Fact]
        public void CompleteConversationExchange_UpdatesAssistantMessageContent()
        {
            var thread = new ConversationThread(0);

            var exchangeId = thread.CreateConversationExchange();

            thread.CompleteConversationExchange(exchangeId, "assistant response");

            var exchange = thread.Exchanges.First(x => x.ExchangeId.Equals(exchangeId));

            Assert.Equal("assistant response", exchange.AssistantMessage.Content);
        }
    }
}

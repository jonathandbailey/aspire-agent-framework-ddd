
using Application.Interfaces;

namespace Agents.Conversation.Interfaces;

public interface IAgentFactory
{
    Task<IAgent> CreateAgent();
}
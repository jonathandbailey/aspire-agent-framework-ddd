import { useCallback } from "react";
import { UIMessageFactory } from "../factories/UIMessageFactory";
import { ConversationService } from "../services/chat/conversation.service";
import { useLiveExchanges } from "../stores/exchange.store";

export const useConversationExchange = (conversationId: string, threadId?: string) => {
    const conversationService = new ConversationService();
    const { addExchange } = useLiveExchanges(threadId ?? "");

    const handlePrompt = useCallback(async (value: string) => {
        const userMessage = UIMessageFactory.createUserMessage(value);
        const assistantMessage = UIMessageFactory.createAssistantMessage();

        const exchangeId = await conversationService.CreateConversationExchange(conversationId);

        addExchange({
            id: exchangeId,
            user: userMessage,
            assistant: assistantMessage,
            isPending: true
        });

        try {
            await conversationService.startConversationExchange(
                value,
                assistantMessage.id,
                conversationId,
                exchangeId
            );
        } catch (err) {
            // Handle error as needed
        }
    }, [conversationId, conversationService, addExchange]);

    return { handlePrompt };
};
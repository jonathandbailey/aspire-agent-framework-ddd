import apiClient from "../../api/client/api-client";
import type { ChatRequestDto } from "../../types/dto/chat-request.dto";
import type { ChatResponseDto } from "../../types/dto/chat-response.dto";
import type { Conversation } from "../../types/models/chat/conversation";
import type { ConversationSummary } from "../../types/models/chat/conversationSummary";

export class ConversationService {

    async LoadConversation(conversationId: string): Promise<Conversation> {
        const response = await apiClient.get<Conversation>(`api/conversations/${conversationId}`, {});
        return response.data;
    }

    async LoadConversationSummaries(): Promise<ConversationSummary[]> {
        const response = await apiClient.get<ConversationSummary[]>(`api/conversations/summaries`, {});
        return response.data;
    }

    async CreateConversation(): Promise<Conversation> {
        const response = await apiClient.post<Conversation>(`api/conversations`);
        return response.data;
    }

    async CreateConversationExchange(conversationId: string): Promise<string> {
        const response = await apiClient.post<string>(`api/conversations/${conversationId}/exchanges`);
        return response.data;
    }

    async startConversationExchange(message: string, id: string, conversationId: string, exchangeId: string): Promise<ChatResponseDto> {


        const request: ChatRequestDto = { message, id, conversationId, exchangeId };

        const response = await apiClient.post<ChatResponseDto>(`api/conversations/${conversationId}/exchanges/${exchangeId}/messages`, request);
        return response.data;
    }
}
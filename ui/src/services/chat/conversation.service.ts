import apiClient from "../../api/client/api-client";
import type { ChatRequestDto } from "../../types/dto/chat-request.dto";
import type { ChatResponseDto } from "../../types/dto/chat-response.dto";
import type { Conversation } from "../../types/models/chat/conversation";

export class ConversationService {

    async LoadConversation(conversationId: string): Promise<Conversation> {
        const response = await apiClient.get<Conversation>(`api/conversations/${conversationId}`, {});
        return response.data;
    }

    async LoadConversations(): Promise<Conversation[]> {
        const response = await apiClient.get<Conversation[]>(`api/conversations`, {});
        return response.data;
    }

    async LoadConversationSummaries(): Promise<Conversation[]> {
        const response = await apiClient.get<Conversation[]>(`api/conversations`, {});
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

        const response = await apiClient.post<ChatResponseDto>('api/chat', request);
        return response.data;
    }
}
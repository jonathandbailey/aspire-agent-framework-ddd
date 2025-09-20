import apiClient from "../../api/client/api-client";
import type { Conversation } from "../../types/models/chat/conversation";

export class ConversationService {

    async LoadConversation(conversationId: string): Promise<Conversation> {
        const response = await apiClient.get<Conversation>(`api/conversation/${conversationId}`, {});
        return response.data;
    }

    async LoadConversations(): Promise<Conversation[]> {
        const response = await apiClient.get<Conversation[]>(`api/conversation`, {});
        return response.data;
    }

    async CreateConversation(): Promise<Conversation> {
        const response = await apiClient.post<Conversation>(`api/conversation`);
        return response.data;
    }
}
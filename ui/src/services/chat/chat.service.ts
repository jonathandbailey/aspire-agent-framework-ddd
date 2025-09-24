import apiClient from "../../api/client/api-client";
import type { ChatResponseDto } from "../../types/dto/chat-response.dto";
import type { ChatRequestDto } from "../../types/dto/chat-request.dto";

export class ChatService {

    async sendMessage(message: string, id: string, conversationId: string, exchangeId: string): Promise<ChatResponseDto> {


        const request: ChatRequestDto = { message, id, conversationId, exchangeId };

        const response = await apiClient.post<ChatResponseDto>('api/chat', request);
        return response.data;
    }
}
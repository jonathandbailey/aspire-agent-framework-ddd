import type { Conversation } from "../../types/models/chat/conversation"
import type { ConversationExchange } from "../../types/models/chat/conversationExchange"
import type { ConversationThread } from "../../types/models/chat/conversationThread"
import type { Message } from "../../types/models/chat/message"
import type { UIConversation } from "../../types/ui/UIConversation"
import type { UIConversationThread } from "../../types/ui/UIConversationThread"
import type { UIExchange } from "../../types/ui/UIExchange"
import type { UIMessage } from "../../types/ui/UIMessage"


export function mapMessage(dto: Message): UIMessage {
    return {
        id: dto.id,
        role: dto.role,
        text: dto.content,
        isLoading: false,
        hasError: false,
        errorMessage: "",
    }
}

export function mapExchange(dto: ConversationExchange): UIExchange {
    return {
        id: dto.id,
        user: mapMessage(dto.userMessage),
        assistant: mapMessage(dto.assistantMessage),
    }
}

export function mapThread(dto: ConversationThread): UIConversationThread {
    return {
        id: dto.id,
        exchanges: dto.exchanges.map(mapExchange),
    }
}

export function mapConversation(dto: Conversation): UIConversation {
    return {
        id: dto.id,
        title: dto.name,
        threads: dto.threads.map(mapThread),
    }
}

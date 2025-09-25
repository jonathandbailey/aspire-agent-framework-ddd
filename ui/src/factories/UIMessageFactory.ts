import type { Message } from "../types/models/chat/message";
import type { UIMessage } from "../types/ui/UIMessage";

export class UIMessageFactory {
    static updateMessage(prev: UIMessage, update: Partial<UIMessage>): UIMessage {
        return {
            ...prev,
            ...update,
        };
    }
    static createUserMessage(text: string): UIMessage {
        return {
            id: crypto.randomUUID(),
            role: 'user',
            text,
            isLoading: true,
            hasError: false,
            errorMessage: "",
        };
    }

    static createMessage(msg: Message) {
        return {
            id: msg.id,
            role: msg.role,
            text: msg.content,
            isLoading: false,
            hasError: false,
            errorMessage: "",
        }
    }

    static createAssistantMessage(): UIMessage {
        return {
            id: crypto.randomUUID(),
            role: 'assistant',
            text: "",
            isLoading: true,
            hasError: false,
            errorMessage: "",
        };
    }
}

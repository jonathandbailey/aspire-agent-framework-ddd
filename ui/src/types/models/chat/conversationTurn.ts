import type { Message } from "./message";

export interface ConversationTurn {
    id: string;
    title: string;
    userMessage: Message;
    assistantMessage: Message;
}
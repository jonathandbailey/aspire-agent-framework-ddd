import type { Message } from "./message";

export interface ConversationThread {
    id: string;
    title: string;
    messages: Message[];
}
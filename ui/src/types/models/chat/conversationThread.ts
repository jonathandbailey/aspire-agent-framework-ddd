import type { ConversationTurn } from "./conversationTurn";

export interface ConversationThread {
    id: string;
    title: string;
    turns: ConversationTurn[];
}
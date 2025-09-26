import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { ConversationService } from "../services/chat/conversation.service";
import type { ConversationSummary } from "../types/models/chat/conversationSummary";

export const CONVERSATIONS_QUERY_KEY = ["conversations"] as const;

export const useConversationSummaries = () => {
    const conversationService = useMemo(() => new ConversationService(), []);

    return useQuery<ConversationSummary[]>({
        queryKey: CONVERSATIONS_QUERY_KEY,
        queryFn: () => conversationService.LoadConversationSummaries(),
        retry: false,
    });
};

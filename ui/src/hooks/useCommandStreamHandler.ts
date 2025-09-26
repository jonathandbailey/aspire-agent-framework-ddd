import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import streamingService from "../services/chat/streaming.service";
import type { Conversation } from "../types/models/chat/conversation";
import { CONVERSATIONS_QUERY_KEY } from "./useConversationSummaries";

export const useCommandStreamHandler = () => {
    const queryClient = useQueryClient();

    useEffect(() => {
        const handleCommand = ({ conversationId, title }: { conversationId: string; title: string }) => {
            queryClient.setQueryData(CONVERSATIONS_QUERY_KEY, (oldConversations: Conversation[] = []) =>
                oldConversations.map(convo =>
                    convo.id === conversationId ? { ...convo, name: title } : convo
                )
            );
            queryClient.invalidateQueries({ queryKey: CONVERSATIONS_QUERY_KEY });
        };

        streamingService.on("command", handleCommand);

        return () => {
            streamingService.off("command", handleCommand);
        };
    }, [queryClient]);
};

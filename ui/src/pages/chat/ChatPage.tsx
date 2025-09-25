import { useState, useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import ChatInput from "../../components/chat/ChatInput";

import type { ChatResponseDto } from "../../types/dto/chat-response.dto";
import streamingService from "../../services/chat/streaming.service";
import { UIMessageFactory } from "../../factories/UIMessageFactory";
import { ConversationService } from "../../services/chat/conversation.service";
import { useParams } from "react-router-dom";
import { Flex } from "antd";
import ConversationComponent from "../../components/chat/Conversation";
import type { UIConversation } from "../../types/ui/UIConversation";
import { useLiveExchanges } from "../../stores/exchange.store";

import {
    mapConversation
} from "./conversationMappers";

const ChatPage = () => {
    const [uiConversation, setUiConversation] = useState<UIConversation | null>(null);
    const [threadId, setThreadId] = useState<string>("");

    const { id } = useParams();

    if (!id) throw new Error("Conversation id is required");

    const conversationId = id;
    const conversationService = new ConversationService();

    const { data: conversation } = useQuery({
        queryKey: ["conversation", conversationId],
        queryFn: () => conversationService.LoadConversation(conversationId),
        enabled: !!conversationId,
    });

    useEffect(() => {
        if (conversation) {
            const mapped = mapConversation(conversation);
            setUiConversation(mapped);
            if (mapped.threads.length > 0) {
                setThreadId(mapped.threads[0].id);
            }
        }
    }, [conversation]);


    const { addExchange, appendToAssistantMessage } =
        useLiveExchanges(threadId)

    streamingService.on("chat", (response: ChatResponseDto) => {
        appendToAssistantMessage(response.id, response.message, false);
    });

    const handlePrompt = async (value: string) => {
        const userMessage = UIMessageFactory.createUserMessage(value);
        const assistantMessage = UIMessageFactory.createAssistantMessage();

        var exchangeId = await conversationService.CreateConversationExchange(conversationId);

        addExchange({
            id: exchangeId, user: userMessage, assistant: assistantMessage, isPending: true
        })

        try {
            await conversationService.startConversationExchange(
                value,
                assistantMessage.id,
                conversationId,
                exchangeId
            );
        } catch (err) {
        }
    };

    return (
        <>

            <Flex vertical>
                {uiConversation && <ConversationComponent conversation={uiConversation} />}

                <div style={{ width: 700, position: 'sticky', bottom: 0, background: '#fff', zIndex: 10 }}>
                    <ChatInput onEnter={handlePrompt} />
                </div>
            </Flex>


        </>

    );
};

export default ChatPage;

import { useState, useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import ChatInput from "../../components/chat/ChatInput";

import type { ChatResponseDto } from "../../types/dto/chat-response.dto";
import streamingService from "../../services/chat/streaming.service";
import type { UIMessage } from "../../types/ui/UIMessage";
import { UIMessageFactory } from "../../factories/UIMessageFactory";
import { ConversationService } from "../../services/chat/conversation.service";
import { useParams } from "react-router-dom";
import { Flex } from "antd";
import ConversationComponent from "../../components/chat/Conversation";
import type { Conversation } from "../../types/models/chat/conversation";
import type { UIConversation } from "../../types/ui/UIConversation";
import type { ConversationThread } from "../../types/models/chat/conversationThread";
import type { UIConversationThread } from "../../types/ui/UIConversationThread";
import type { UIExchange } from "../../types/ui/UIExchange";
import type { Message } from "../../types/models/chat/message";
import type { ConversationExchange } from "../../types/models/chat/conversationExchange";

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

function mapThread(dto: ConversationThread): UIConversationThread {
    return {
        id: dto.id,
        exchanges: dto.exchanges.map(mapExchange),
    }
}

function mapConversation(dto: Conversation): UIConversation {
    return {
        id: dto.id,
        title: dto.name,
        threads: dto.threads.map(mapThread),
    }
}

const ChatPage = () => {
    const [messages, setMessages] = useState<UIMessage[]>([]);
    const [responseMessage, setResponseMessage] = useState<UIMessage | null>(null);
    const [uiConversation, setUiConversation] = useState<UIConversation | null>(null);

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


            setUiConversation(mapConversation(conversation));
        }
    }, [conversation]);

    streamingService.on("chat", (response: ChatResponseDto) => {

        setMessages((prev) =>
            prev.map((msg) =>
                msg.id === responseMessage?.id
                    ? UIMessageFactory.updateMessage(msg, {
                        text: msg.text + response.message,
                        isLoading: false,
                        hasError: false,
                        errorMessage: "",
                    })
                    : msg
            )
        );
    });

    const handlePrompt = async (value: string) => {
        const userMessage = UIMessageFactory.createUserMessage(value);
        setMessages(prev => [...prev, userMessage]);

        const assistantMessage = UIMessageFactory.createAssistantMessage();
        setMessages(prev => [...prev, assistantMessage]);

        setResponseMessage(assistantMessage);

        var exchangeId = await conversationService.CreateConversationExchange(conversationId);

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

            <div style={{ display: "flex", flexDirection: "column", height: "95vh" }}>
                <Flex>
                    {uiConversation && <ConversationComponent conversation={uiConversation} />}
                </Flex>

                <div style={{ width: 700, position: 'sticky', bottom: 0, background: '#fff', zIndex: 10 }}>
                    <ChatInput onEnter={handlePrompt} />
                </div>
            </div>
        </>

    );
};

export default ChatPage;

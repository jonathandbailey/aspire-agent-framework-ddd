import { useState, useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import { ChatService } from "../../services/chat/chat.service";
import ChatInput from "../../components/chat/ChatInput";

import type { ChatResponseDto } from "../../types/dto/chat-response.dto";
import streamingService from "../../services/chat/streaming.service";
import type { UIMessage } from "../../types/ui/UIMessage";
import { UIMessageFactory } from "../../factories/UIMessageFactory";
import { ConversationService } from "../../services/chat/conversation.service";
import { useParams } from "react-router-dom";
import ChatConversation from "../../components/chat/ChatConversation";

const ChatPage = () => {
    const [messages, setMessages] = useState<UIMessage[]>([]);
    const [responseMessage, setResponseMessage] = useState<UIMessage | null>(null);
    const { id } = useParams();

    if (!id) throw new Error("Conversation id is required");

    const conversationId = id;
    const chatService = new ChatService();

    const conversationService = new ConversationService();

    const { data: conversation } = useQuery({
        queryKey: ["conversation", conversationId],
        queryFn: () => conversationService.LoadConversation(conversationId),
        enabled: !!conversationId,
    });

    useEffect(() => {
        if (conversation) {
            const allMessages = conversation.threads.flatMap(thread =>
                thread.exchanges.flatMap(turn => [
                    UIMessageFactory.createMessage(turn.userMessage),
                    UIMessageFactory.createMessage(turn.assistantMessage)
                ])
            );
            setMessages(allMessages);
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
            await chatService.sendMessage(
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
                <ChatConversation messages={messages} />
                <div style={{ width: 700, position: 'sticky', bottom: 0, background: '#fff', zIndex: 10 }}>
                    <ChatInput onEnter={handlePrompt} />
                </div>
            </div>
        </>

    );
};

export default ChatPage;

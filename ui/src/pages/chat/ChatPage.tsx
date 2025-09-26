import { useQuery } from "@tanstack/react-query";
import ChatInput from "../../components/chat/ChatInput";

import type { ChatResponseDto } from "../../types/dto/chat-response.dto";
import streamingService from "../../services/chat/streaming.service";
import { ConversationService } from "../../services/chat/conversation.service";
import { useParams } from "react-router-dom";
import { Alert, Flex, Skeleton } from "antd";
import ConversationComponent from "../../components/chat/Conversation";
import { useLiveExchanges } from "../../stores/exchange.store";
import { useConversationExchange } from "../../hooks/useConversationExchange";

import { mapConversation } from "./conversationMappers";

const ChatPage = () => {

    const { id } = useParams();

    if (!id) throw new Error("Conversation id is required");

    const conversationId = id;
    const conversationService = new ConversationService();

    const { data: conversation, isLoading, isError, error } = useQuery({
        queryKey: ["conversation", conversationId],
        queryFn: () => conversationService.LoadConversation(conversationId),
        enabled: !!conversationId,
        select: mapConversation,
    });



    const { appendToAssistantMessage } =
        useLiveExchanges(conversation?.threads[0]?.id ?? "")

    const { handlePrompt } = useConversationExchange(
        conversationId,
        conversation?.threads[0]?.id
    );

    streamingService.on("chat", (response: ChatResponseDto) => {
        appendToAssistantMessage(response.id, response.message, false);
    });


    if (isLoading) {
        return <Skeleton style={{ width: 700, height: 400 }} />;
    }

    if (isError) {
        return <Alert
            message="Failed to load conversation"
            description={(error as Error).message}
            type="error"
            showIcon
        />
    }



    return (
        <>
            <Flex vertical >
                {conversation && <ConversationComponent conversation={conversation} />}

                <div style={{ width: 700, position: 'sticky', bottom: 0, background: '#fff', zIndex: 10 }}>
                    <ChatInput onEnter={handlePrompt} />
                </div>
            </Flex>

        </>
    );
};

export default ChatPage;

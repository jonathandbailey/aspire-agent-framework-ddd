import { Flex, List } from "antd";
import UserMessage from "./UserMessage";
import AssistantMessage from "./AssistantMessage";
import type { UIMessage } from "../../types/ui/UIMessage";
import { useEffect, useRef, useState } from "react";


interface ChatConversationProps {
    messages: UIMessage[];
}

const ChatConversation = ({ messages }: ChatConversationProps) => {
    const [uiMessages, setUiMessages] = useState<UIMessage[]>([]);

    const lastMessageRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        setUiMessages(messages);

        if (lastMessageRef.current) {
            lastMessageRef.current.scrollIntoView({ behavior: "smooth" });
        }
    }, [messages]);

    return (
        <>
            <List style={{ flex: 1, overflowY: "auto", padding: "48px", width: 700 }} split={false}>
                {uiMessages.map((message, idx) => {
                    const isLast = idx === uiMessages.length - 1;
                    return (
                        <List.Item key={idx} style={{}} ref={isLast ? lastMessageRef : null}>
                            <Flex vertical={true} style={{ width: "100%" }}>
                                {message.role === "user" ? (
                                    <Flex justify="flex-end" style={{ width: "100%" }}>
                                        <UserMessage message={message} />
                                    </Flex>
                                ) : (
                                    <Flex style={{ width: "100%" }}>
                                        <AssistantMessage message={message} />
                                    </Flex>
                                )}
                            </Flex>
                        </List.Item>
                    );
                })}
            </List>
        </>
    );
}

export default ChatConversation;

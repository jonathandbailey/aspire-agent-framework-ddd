import { Flex } from "antd";
import type { UIConversationThread } from "../../types/ui/UIConversationThread";
import AssistantMessage from "./AssistantMessage";
import UserMessage from "./UserMessage";
import { useLiveExchanges } from "../../stores/exchange.store";

interface ConversationThreadProps {
    thread: UIConversationThread;
}

const ConversationThread = ({ thread }: ConversationThreadProps) => {
    const { exchanges: liveExchanges } =
        useLiveExchanges(thread.id)
    return (
        <>
            {thread.exchanges.map((exchange, idx) => (
                <div key={idx}>
                    <Flex justify="flex-end" style={{ width: "100%" }}>
                        <UserMessage message={exchange.user} />
                    </Flex>


                    <AssistantMessage message={exchange.assistant} />
                </div>
            ))}
            {liveExchanges.map((exchange, idx) => (
                <div key={idx}>
                    <Flex justify="flex-end" style={{ width: "100%" }}>
                        <div><UserMessage message={exchange.user} /></div>
                    </Flex>
                    <div><AssistantMessage message={exchange.assistant} /></div>
                </div>


            ))}



        </>);
}

export default ConversationThread;
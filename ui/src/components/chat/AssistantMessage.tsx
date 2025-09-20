import { Alert, Card } from "antd";
import Markdown from "react-markdown";
import type { UIMessage } from "../../types/ui/UIMessage";
import styles from "./AssistantMessage.module.css";

interface AssistantMessageProps {
    message: UIMessage
}

const AssistantMessage = ({ message }: AssistantMessageProps) => {
    return (
        <Card
            className={styles["assistant-message-card"]}
            variant="borderless"
            loading={message.isLoading}
        >
            {message.hasError ? (
                <Alert
                    message={message.errorMessage}
                    description="There was an error processing your request. Please try again."
                    type="error"
                    showIcon
                />
            ) : (
                <Markdown>{message.text}</Markdown>
            )}
        </Card>
    );
};

export default AssistantMessage;